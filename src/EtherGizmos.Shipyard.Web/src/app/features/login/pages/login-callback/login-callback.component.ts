import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OAuth2Service } from '../../../../shared/services/oauth2/oauth2.service';

@Component({
  selector: 'app-login-callback',
  imports: [],
  templateUrl: './login-callback.component.html',
  styleUrl: './login-callback.component.scss'
})
export class LoginCallbackComponent implements OnInit {

  private readonly $oauth2 = inject(OAuth2Service);
  private readonly $router = inject(Router);

  async ngOnInit() {
    await this.$oauth2.onReady;
    this.$router.navigate(["/"]);
  }
}
