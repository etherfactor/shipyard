import { Component, inject } from '@angular/core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { openOffcanvas } from '../../../../shared/util/offcanvas/offcanvas.util';
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

  private readonly $offcanvas = inject(NgbOffcanvas);

  async openSidebar() {
    await openOffcanvas(this.$offcanvas, SidebarComponent);
  }
}
