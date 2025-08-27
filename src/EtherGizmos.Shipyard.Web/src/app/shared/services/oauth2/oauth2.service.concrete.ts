import { effect, inject, Injectable, Provider, signal } from "@angular/core";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { OAuth2Service } from "./oauth2.service";

@Injectable({
  providedIn: 'root'
})
export class ConcreteOAuth2Service extends OAuth2Service {

  private readonly $oidc = inject(OidcSecurityService);

  private onReadyResolve!: () => void;
  readonly onReady = new Promise<void>((resolve, reject) => {
    this.onReadyResolve = resolve;
  });
  readonly isReady$$ = signal(false);
  readonly accessToken$$ = signal("");
  readonly idToken$$ = signal("");
  readonly idTokenData$$ = signal({} as any);

  constructor() {
    super();
    this.$oidc.checkAuth().subscribe();

    effect(() => {
      const auth = this.$oidc.authenticated();
      this.$oidc.getAccessToken().subscribe(token => this.accessToken$$.set(token));
      this.$oidc.getIdToken().subscribe(token => this.idToken$$.set(token));
      if (auth.isAuthenticated) {
        this.$oidc.getPayloadFromIdToken().subscribe(token => this.idTokenData$$.set(token));
        this.onReadyResolve();
      } else {
        this.idTokenData$$.set({});
      }
    });
  }

  login(): void {
    this.$oidc.authorize();
  }

  logout(): void {
    this.$oidc.logoffAndRevokeTokens().subscribe();
    this.$oidc.logoffLocal();
  }
}

export function provideOAuth2Service(): Provider {
  return {
    provide: OAuth2Service,
    useClass: ConcreteOAuth2Service,
  };
}
