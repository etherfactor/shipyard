import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserSessionService } from '../../../features/login/services/user-session/user-session.service';
import { PermissionId } from '../../../features/security/models/permission-id';
import { SecurableType } from '../../../features/security/models/securable-type';
import { Logger } from '../../utilities/logger/logger.util';
import { OAuth2Service } from '../../services/oauth2/oauth2.service';

export function authorizationGuard(securableType: SecurableType, permissionId: PermissionId): CanActivateFn {
  return async (route, state) => {
    const $oauth2 = inject(OAuth2Service);
    const $session = inject(UserSessionService);
    const $router = inject(Router);
    const $logger = inject(Logger).forContext("authorizationGuard");

    await $oauth2.onReady;

    const hasCapability = $session.hasCapability(securableType, permissionId);
    if (!hasCapability) {
      $logger.warning(
        "User tried to access a route, but they lacked the permission: {SecurableType}:{PermissionId}",
        securableType,
        permissionId);

      $router.navigate(["/403"]);
    }

    return hasCapability;
  }
};
