import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './features/app/components/navbar/navbar.component';
import { OAuth2Service } from './shared/services/oauth2/oauth2.service';

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

  private readonly $oauth2 = inject(OAuth2Service);

  title = 'EtherGizmos.Shipyard.Web';
}
