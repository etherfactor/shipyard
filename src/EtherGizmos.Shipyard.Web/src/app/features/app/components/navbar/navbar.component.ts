import { Component, inject } from '@angular/core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';
import { openOffcanvas } from '../../../../shared/utilities/offcanvas/offcanvas.util';
import { NavbarActionComponent } from '../navbar-action/navbar-action.component';
import { NavbarBreadcrumbComponent } from '../navbar-breadcrumb/navbar-breadcrumb.component';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-navbar',
  imports: [
    NavbarActionComponent,
    NavbarBreadcrumbComponent,
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  private readonly $oauth2 = inject(OAuth2Service);
  private readonly $offcanvas = inject(NgbOffcanvas);

  async openSidebar() {
    await openOffcanvas(this.$offcanvas, SidebarComponent);
  }

  login(): void {
    this.$oauth2.login();
  }

  logout(): void {
    this.$oauth2.logout();
  }
}
