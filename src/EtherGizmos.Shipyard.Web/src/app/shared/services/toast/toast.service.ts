import { Injectable, signal } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class ToastService {
  private readonly selfToasts$$ = signal<ToastInfo[]>([]);
  readonly toasts$$ = this.selfToasts$$.asReadonly();

  show(toast: ToastInfo) {
    if (toast.id) {
      this.hide(toast.id);
    }

    this.selfToasts$$.set([...this.selfToasts$$(), toast]);
  }

  hide(id: string): void;
  hide(toast: ToastInfo): void;
  hide(arg1: ToastInfo | string) {
    if (typeof arg1 === "string") {
      this.selfToasts$$.set(this.selfToasts$$().filter(t => t.id !== arg1));
    } else {
      this.selfToasts$$.set(this.selfToasts$$().filter(t => t !== arg1));
    }
  }
}

type ToastTheme = "primary" | "secondary" | "success" | "warning" | "danger" | "info";

export interface ToastInfo {
  id?: string;
  header: string;
  body: string;
  theme?: ToastTheme;
  delay?: number;
  actions?: ToastAction[];
}

export interface ToastAction {
  label: string;
  color?: string;
  execute: () => void;
}
