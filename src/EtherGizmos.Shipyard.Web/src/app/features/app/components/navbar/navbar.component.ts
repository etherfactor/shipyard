import { Component, computed, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { buildUrl } from '@ethergizmos/odata-fluent-client/dist/src/utils/http';
import { NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { Options } from '@popperjs/core';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { NavbarActionComponent } from '../navbar-action/navbar-action.component';
import { NavbarBreadcrumbComponent } from '../navbar-breadcrumb/navbar-breadcrumb.component';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-navbar',
  imports: [
    NavbarActionComponent,
    NavbarBreadcrumbComponent,
    NgbDropdownModule,
    RouterModule,
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  private readonly $oauth2 = inject(OAuth2Service);
  private readonly $offcanvas = inject(NgbOffcanvas);
  private readonly $userSession = inject(UserSessionService);
  private readonly config = inject(APP_CONFIG);

  readonly isSignedIn$$ = computed(() => this.$userSession.isSignedIn$$());
  readonly name$$ = computed(() => this.$userSession.navbarName$$());

  private sidebar?: SidebarComponent;

  async openSidebar() {
    if (this.sidebar) {
      this.sidebar.close(0);
    } else {
      const offcanvas = this.$offcanvas.open(SidebarComponent, { panelClass: "sidebar" });
      try {
        this.sidebar = offcanvas.componentInstance;
        await offcanvas.result;
      } finally {
        this.sidebar = undefined;
      }
    }
  }

  login(): void {
    this.$oauth2.login();
  }

  logout(): void {
    this.$oauth2.logout();
  }

  protected get changePasswordUrl() {
    return buildUrl(this.config.oauth.authority, "account", `change-password?returnUrl=${window.location.href}`);
  }

  protected popperOptions(opts: Partial<Options>): Partial<Options> {
    return { ...opts, strategy: "absolute" };
  }
}
