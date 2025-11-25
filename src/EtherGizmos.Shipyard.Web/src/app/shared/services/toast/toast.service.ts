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

export interface ToastInfo {
  header: string;
  body: string;
  delay?: number;
  actions?: ToastAction[];
}

export interface ToastAction {
  label: string;
  color?: string;
  execute: () => void;
}
