import { Component, computed, inject } from '@angular/core';
import { NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { Options } from '@popperjs/core';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';
import { openOffcanvas } from '../../../../shared/utilities/offcanvas/offcanvas.util';
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
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  private readonly $oauth2 = inject(OAuth2Service);
  private readonly $offcanvas = inject(NgbOffcanvas);
  private readonly $userSession = inject(UserSessionService);

  readonly isSignedIn$$ = computed(() => this.$userSession.isSignedIn$$());
  readonly name$$ = computed(() => this.$userSession.navbarName$$());

  async openSidebar() {
    await openOffcanvas(this.$offcanvas, SidebarComponent);
  }

  login(): void {
    this.$oauth2.login();
  }

  logout(): void {
    this.$oauth2.logout();
  }

  protected popperOptions(opts: Partial<Options>): Partial<Options> {
    return { ...opts, strategy: "absolute" };
  }
}
