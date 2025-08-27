import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-callback',
  imports: [],
  templateUrl: './login-callback.component.html',
  styleUrl: './login-callback.component.scss'
})
export class LoginCallbackComponent implements OnInit {

  private readonly $router = inject(Router);

  ngOnInit(): void {
    this.$router.navigate(["/"]);
  }
}
