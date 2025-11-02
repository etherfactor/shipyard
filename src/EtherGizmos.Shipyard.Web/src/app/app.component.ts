import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './features/app/components/navbar/navbar.component';
import { UserSessionService } from './features/login/services/user-session/user-session.service';
import { OAuth2Service } from './shared/services/oauth2/oauth2.service';
import { Logger } from './shared/utilities/logger/logger.util';

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
export class AppComponent implements OnInit {

  private readonly $logger = inject(Logger).forContext("AppComponent");
  private readonly $oauth2 = inject(OAuth2Service);
  private readonly $session = inject(UserSessionService);

  readonly isSignedIn$$ = computed(() => this.$session.isSignedIn$$());

  title = 'EtherGizmos.Shipyard.Web';

  async ngOnInit() {
    this.$logger.information("App initializing");

    await this.$oauth2.onReady;

    this.$logger.information("App initialized");
  }
}
