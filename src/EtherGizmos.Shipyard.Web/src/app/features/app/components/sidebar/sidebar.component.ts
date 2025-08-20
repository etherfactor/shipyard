import { Component, computed, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { InitializeOffcanvas } from '../../../../shared/components/initialize-offcanvas/initialize-offcanvas.component';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent extends InitializeOffcanvas<[], 0> {

  private readonly config = inject(APP_CONFIG);

  version$$ = computed(() => this.config.version);

  override get defaultDismiss(): 0 {
    return 0;
  }

  override initialize(): void { }
}
