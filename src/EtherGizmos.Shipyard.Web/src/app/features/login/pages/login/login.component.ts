import { Component, inject } from '@angular/core';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  private readonly $oauth2 = inject(OAuth2Service);
  private readonly config = inject(APP_CONFIG);

  get version() {
    return this.config.version;
  }

  login() {
    this.$oauth2.login();
  }
}
