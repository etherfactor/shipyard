import { Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { NavbarAction } from '../../../features/app/components/navbar-action/navbar-action.component';

@Injectable({
  providedIn: 'root'
})
export class NavbarActionService {

  private actionsInternal$$: WritableSignal<NavbarAction[]>;

  get actions$$(): Signal<NavbarAction[]> {
    return this.actionsInternal$$;
  }

  constructor() {
    this.actionsInternal$$ = signal([]);
  }

  setActions(actions: NavbarAction[]) {
    this.actionsInternal$$.set(actions);
  }
}
