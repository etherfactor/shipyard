import { Component, computed, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { InitializeOffcanvas } from '../../../../shared/components/initialize-offcanvas/initialize-offcanvas.component';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent extends InitializeOffcanvas<[], 0> {

  private readonly config = inject(APP_CONFIG);
  private readonly $session = inject(UserSessionService);

  version$$ = computed(() => this.config.version);

  override get defaultDismiss(): 0 {
    return 0;
  }

  override initialize(): void { }

  hasCapability(securableType: SecurableType, permissionId: PermissionId) {
    return this.$session.hasCapability(securableType, permissionId);
  }

  SecurableType = SecurableType;
  PermissionId = PermissionId;
}
