import { Component, computed, inject } from "@angular/core";
import { NgbToast } from "@ng-bootstrap/ng-bootstrap";
import { ToastInfo, ToastService } from "../../../../shared/services/toast/toast.service";

@Component({
  selector: "app-toast",
  imports: [
    NgbToast,
  ],
  templateUrl: "./toast.component.html",
  styleUrl: "./toast.component.scss",
})
export class ToastComponent {
  private readonly $toast = inject(ToastService);

  readonly toasts$$ = computed(() => this.$toast.toasts$$());

  hide(id: string): void;
  hide(toast: ToastInfo): void;
  hide(arg1: ToastInfo | string) {
    //Why...?
    if (typeof arg1 === "string") {
      this.$toast.hide(arg1);
    } else {
      this.$toast.hide(arg1);
    }
  }
}
