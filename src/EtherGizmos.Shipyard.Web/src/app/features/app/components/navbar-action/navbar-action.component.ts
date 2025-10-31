import { Component, effect, Signal, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter, timer } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { NavbarSearchActionDirective } from '../../directives/navbar-search-action.directive';

@Component({
  selector: 'app-navbar-action',
  imports: [
    NavbarSearchActionDirective,
  ],
  templateUrl: './navbar-action.component.html',
  styleUrl: './navbar-action.component.scss'
})
export class NavbarActionComponent {

  private $navbarAction: NavbarActionService;
  private $router: Router;

  actions: NavbarAction[] = [];

  constructor(
    $navbarAction: NavbarActionService,
    $router: Router,
  ) {
    this.$navbarAction = $navbarAction;
    this.$router = $router;

    $router.events.pipe(
      filter(event => event instanceof NavigationEnd),
    ).subscribe(() => {
      this.delaySetActions([]);
    });

    effect(() => {
      this.delaySetActions(this.$navbarAction.actions$$());
    });
  }

  private delaySetActions(actions: NavbarAction[]) {
    timer(0).subscribe(() => {
      this.actions = actions;
    });
  }

  safe(input: string | Signal<string> | undefined): Signal<string | undefined> {
    if (typeof input === "string" || !input) {
      return signal(input);
    }

    return input;
  }

  performCallback(action: NavbarCallback) {

    //const searchAction = this.searchActions.find(e => e.searchAction === action);
    //if (searchAction) {
    //  searchAction.focus();
    //}

    action.callback?.();
  }

  identifyNavbarAction(action: NavbarAction) {
    return action.label;
  }

  identifyNavbarSubAction(action: NavbarSubAction) {
    return action.label ?? uuidv4();
  }
}

export interface NavbarCallback {
  callback?: () => any;
}

export interface NavbarAction extends NavbarCallback {
  label: string | Signal<string>;
  icon?: string;
  subActionSearch?: boolean;
  subActions?: NavbarSubAction[];
}

export interface NavbarSubAction extends NavbarCallback {
  label?: string | Signal<string>;
  icon?: string;
  divider?: boolean;
}
