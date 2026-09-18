import { effect, inject, Injectable, Provider, signal } from "@angular/core";
import { Router } from "@angular/router";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { catchError, filter, firstValueFrom, of, switchMap, take, throwError, timeout, timer } from "rxjs";
import { Logger } from "../../utilities/logger/logger.util";
import { OAuth2Service } from "./oauth2.service";

@Injectable({
  providedIn: 'root'
})
export class ConcreteOAuth2Service extends OAuth2Service {

  private readonly $logger = inject(Logger).forContext("OAuth2Service");
  private readonly $oidc = inject(OidcSecurityService);
  private readonly $router = inject(Router);

  private oidcReady$$ = signal(false);
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

    this.initialize();

    effect(async () => {
      const auth = this.$oidc.authenticated();
      const oidcReady = this.oidcReady$$();

      this.accessToken$$.set(await firstValueFrom(this.$oidc.getAccessToken()));

      this.idToken$$.set(await firstValueFrom(this.$oidc.getIdToken()));

      let idTokenData: object;
      if (auth.isAuthenticated) {
        idTokenData = await firstValueFrom(this.$oidc.getPayloadFromIdToken());
      } else {
        idTokenData = {};
      }
      this.idTokenData$$.set(idTokenData);

      if (oidcReady) {
        this.onReadyResolve();
      }
    });
  }

  private async initialize() {
    this.$logger.information("OAuth 2.0 initializing...");
    const refreshToken = await firstValueFrom(this.$oidc.getRefreshToken());

    if (refreshToken) {
      this.$logger.information("Found a refresh token");
      let check = this.$oidc.checkAuthIncludingServer();  
      check = check.pipe(
        catchError(err => {
          const message: unknown = err.message;
          if (typeof message === "string" && message.includes("please login")) {
            this.logout();
            return throwError(() => err);
          }

          return timer(1000).pipe(
            switchMap(() => check),
          );
        }),
      );
      const response = await firstValueFrom(check);
      if (response.isAuthenticated) {
        //If the session is authenticated, it will function as expected
        this.$logger.information("Session is currently authenticated");
        this.oidcReady$$.set(true);
      } else {
        this.$logger.information("Session is not currently authenticated; will wait for the next access token");
        const nowAccessToken = response.accessToken;

        //The session expired, so we want to wait for the new access token to come back
        timer(0, 200).pipe(
          switchMap(() => this.$oidc.getAccessToken()),
          filter(token => token !== nowAccessToken),
          timeout(60000),
          catchError(() => of("")),
          take(1),
        ).subscribe(() => {
          //We now have the access token and are ready to go
          this.$logger.information("Found a new access token");
          this.oidcReady$$.set(true);
        });
      }
    } else {
      this.$logger.information("No refresh token found");
      await firstValueFrom(this.$oidc.checkAuth());

      //There's no session, so we can start whenever
      this.oidcReady$$.set(true);
    }
  }

  login(): void {
    this.$oidc.authorize();
  }

  logout(): void {
    this.$router.navigate(["/logout"]);

    this.$oidc.logoffAndRevokeTokens().subscribe();
    this.$oidc.logoffLocal();

    this.$router.navigate(["/login"]);
  }
}

export function provideOAuth2Service(): Provider {
  return {
    provide: OAuth2Service,
    useClass: ConcreteOAuth2Service,
  };
}
