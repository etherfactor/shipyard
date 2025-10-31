import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserSessionService } from '../../../features/login/services/user-session/user-session.service';
import { OAuth2Service } from '../../services/oauth2/oauth2.service';

export const authenticationGuard: CanActivateFn = async (route, state) => {
  const $oauth2 = inject(OAuth2Service);
  const $session = inject(UserSessionService);
  const $router = inject(Router);

  await $oauth2.onReady;
  const signedIn = $session.isSignedIn$$();
  if (!signedIn) {
    $router.navigate(["/login"]);
  }

  return signedIn;
};
