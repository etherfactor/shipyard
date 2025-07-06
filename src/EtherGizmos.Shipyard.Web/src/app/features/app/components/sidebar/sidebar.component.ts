import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { InitializeOffcanvas } from '../../../../shared/components/initialize-offcanvas/initialize-offcanvas.component';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent extends InitializeOffcanvas<[], 0> {

  override get defaultDismiss(): 0 {
    return 0;
  }

  override initialize(): void { }
}
