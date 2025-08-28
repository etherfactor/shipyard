import { Component, inject } from '@angular/core';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  private readonly $oauth2 = inject(OAuth2Service);

  login() {
    this.$oauth2.login();
  }
}
