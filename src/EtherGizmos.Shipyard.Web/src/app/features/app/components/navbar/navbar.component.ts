import { Component } from '@angular/core';
import { NavbarActionComponent } from '../navbar-action/navbar-action.component';
import { NavbarBreadcrumbComponent } from '../navbar-breadcrumb/navbar-breadcrumb.component';

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

}
