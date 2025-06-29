import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './features/app/components/navbar/navbar.component';

@Component({
  selector: 'app-root',
  imports: [
    CommonModule,
    NavbarComponent,
    RouterModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'EtherGizmos.Shipyard.Web';
}
