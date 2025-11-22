import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserSessionService } from '../../../features/login/services/user-session/user-session.service';
import { PermissionId } from '../../../features/security/models/permission-id';
import { SecurableType } from '../../../features/security/models/securable-type';

export function authorizationGuard(securableType: SecurableType, permissionId: PermissionId): CanActivateFn {
  return (route, state) => {
    const $session = inject(UserSessionService);
    const $router = inject(Router);

    const hasCapability = $session.hasCapability(securableType, permissionId);
    if (!hasCapability) {
      $router.navigate(["/403"]);
    }

    return hasCapability;
  }
};
