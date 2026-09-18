import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {

  private readonly selfToasts$$ = signal<ToastInfo[]>([]);
  readonly toasts$$ = this.selfToasts$$.asReadonly();

  show(toast: ToastInfo) {
    this.selfToasts$$.set([...this.selfToasts$$(), toast]);
  }

  hide(toast: ToastInfo) {
    this.selfToasts$$.set(this.selfToasts$$().filter(t => t !== toast));
  }
}

type ToastTheme = "primary" | "secondary" | "success" | "warning" | "danger" | "info";

export interface ToastInfo {
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
