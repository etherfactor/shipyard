import { computed, inject, Injectable } from '@angular/core';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';

@Injectable({
  providedIn: 'root'
})
export class UserSessionService {

  private readonly $oauth2 = inject(OAuth2Service);

  readonly claims$$ = computed<Record<string, any>>(() => this.$oauth2.idTokenData$$());

  readonly person$$ = computed<NameParts | undefined>(() => {
    const claims = this.claims$$();
    return {
      handle: claims["username"],
      given: claims["given_name"],
      family: claims["family_name"],
      full: claims["name"],
    };
  });

  readonly displayName$$ = computed<string | undefined>(() => {
    const person = this.person$$();
    return person
      ? formatName(person, "display")
      : undefined;
  });

  readonly navbarName$$ = computed<string | undefined>(() => {
    const person = this.person$$();
    return person
      ? formatName(person, "short", { useNickname: true })
      : undefined;
  });

  readonly informalName$$ = computed<string | undefined>(() => {
    const person = this.person$$();
    return person
      ? formatName(person, "informal")
      : undefined;
  });

  readonly avatarInitials$$ = computed<string | undefined>(() => {
    const person = this.person$$();
    return person
      ? formatName(person, "initials")
      : undefined;
  });

  readonly isSignedIn$$ = computed<boolean>(() => {
    return !!this.$oauth2.accessToken$$();
  });

  readonly capabilities$$ = computed(() => {
    const raw: string = this.claims$$()["cap"] ?? "";
    const list = raw.split(";")
      .filter(e => !!e)
      .map(e => e.split(":"))
      .map(e => [e[0], bitSplit(Number(e[1]))]);

    const result: Record<SecurableType, PermissionId[]> = Object.fromEntries(list);
    return result;
  });

  hasCapability(securableType: SecurableType, permissionId: PermissionId) {
    const capabilities = this.capabilities$$();
    const forType = capabilities[securableType];
    if (forType && forType.indexOf(permissionId) >= 0) {
      return true;
    } else {
      return false;
    }
  }
}

function bitSplit(value: number) {
  const result: number[] = [];
  let powerOfTwo = 1;
  while (value > 0) {
    if (value & 1) {
      result.push(powerOfTwo);
    }

    value = value >> 1;
    powerOfTwo *= 2;
  }

  return result;
}

interface NameParts {
  given?: string;    // "Jane"
  family?: string;   // "Doe"
  nickname?: string;
  full?: string;     // "Jane Doe"
  handle?: string;   // "jdoe"
}

type NameVariant =
  | "display"  // "Jane Doe" (or best-available)
  | "informal" // "Jane" or nickname
  | "short"    // "Jane D." / "Doe J." (locale-aware order)
  | "initials" // "JD" (avatar chip)
  | "handle"   // preferred_username or email local part
  ;

interface NameFormatOptions {
  locale?: string;
  order?: "western" | "eastern" | "auto";
  useNickname?: boolean;
}

const FAMILY_FIRST = new Set(["ja", "ko", "zh"]);

function inferNameOrder(
  locale: string,
  order?: "western" | "eastern" | "auto",
): "western" | "eastern" {
  if (order && order !== "auto") return order;
  const language = locale.split("-")[0];
  if (FAMILY_FIRST.has(language)) {
    return "eastern";
  } else {
    return "western";
  }
}

function trim(input: string | undefined) { return input?.trim(); }

function initial(input: string | undefined) { return input?.trim()?.[0]?.toUpperCase(); }

function joinNonEmpty(...input: Array<string | undefined>): string {
  return input.filter(e => !!e).join(" ").trim();
}

export function formatName(
  parts: NameParts,
  variant: NameVariant,
  options?: NameFormatOptions,
): string {
  const locale = options?.locale ?? "";
  const order = inferNameOrder(locale, options?.order ?? "auto");

  const useNickname = options?.useNickname ?? true;

  const given = trim(parts.given);
  const family = trim(parts.family);
  const full = trim(parts.full);
  const nickname = trim(parts.nickname);

  switch (variant) {
    case "display":
      const constructed = order === "eastern"
        ? joinNonEmpty(family, given)
        : joinNonEmpty(given, family);
      return full ?? constructed ?? parts.handle ?? "User";

    case "informal":
      return nickname ?? given ?? full ?? parts.handle ?? "User";

    case "short":
      const sgInit = initial(given);
      const sfInit = initial(family);
      if (order === "eastern") {
        if (family && sgInit) return `${family} ${sgInit}`;
      } else {
        if (given && sfInit) return `${given} ${sfInit}`;
      }
      return full ?? parts.handle ?? "User";

    case "initials":
      const igInit = initial(given ?? full?.split(/\s+/)[0]);
      const ifInit = initial(family ?? full?.split(/\s+/).splice(-1)[0]);
      if (order === "eastern") {
        const inits = (igInit ?? "") + (ifInit ?? "");
        if (inits) return inits;
      } else {
        const inits = (ifInit ?? "") + (igInit ?? "");
        if (inits) return inits;
      }
      return "U";

    case "handle":
      return parts.handle ?? "user";
  }
}
