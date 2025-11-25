import { Component, computed, inject } from '@angular/core';
import { NgbToast } from '@ng-bootstrap/ng-bootstrap';
import { ToastInfo, ToastService } from '../../../../shared/services/toast/toast.service';

@Component({
  selector: 'app-toast',
  imports: [
    NgbToast,
  ],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss',
})
export class ToastComponent {

  private readonly $toast = inject(ToastService);

  readonly toasts$$ = computed(() => this.$toast.toasts$$());

  hide(toast: ToastInfo) {
    this.$toast.hide(toast);
  }
}
