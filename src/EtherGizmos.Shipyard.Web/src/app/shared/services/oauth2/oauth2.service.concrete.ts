import { effect, inject, Injectable, Provider, signal } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { OAuth2Service } from "./oauth2.service";

@Injectable({
  providedIn: 'root'
})
export class ConcreteOAuth2Service extends OAuth2Service {

  private readonly $oidc = inject(OidcSecurityService);

  readonly isReady$$ = signal(false);
  readonly accessToken$$ = signal("");
  readonly idToken$$ = signal("");
  readonly idTokenData$$ = signal({} as any);

  constructor() {
    super();
    this.$oidc.checkAuth().subscribe();

    effect(() => {
      this.$oidc.authenticated();
      this.$oidc.getAccessToken().subscribe(token => this.accessToken$$.set(token));
      this.$oidc.getIdToken().subscribe(token => this.idToken$$.set(token));
    });
  }

  login(): void {
    this.$oidc.authorize();
  }

  logout(): void {
    this.$oidc.logoffAndRevokeTokens().subscribe();
  }
}

export function provideOAuth2Service(): Provider {
  return {
    provide: OAuth2Service,
    useClass: ConcreteOAuth2Service,
  };
}
