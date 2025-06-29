import { Directive, ElementRef, Input } from '@angular/core';
import { NavbarAction } from '../components/navbar-action/navbar-action.component';

@Directive({
  selector: '[searchAction]'
})
export class NavbarSearchActionDirective {

  @Input({ required: true }) searchAction: NavbarAction = undefined!;

  private $element: ElementRef;

  constructor(
    $element: ElementRef,
  ) {
    this.$element = $element;
  }

  focus() {
    this.$element.nativeElement.value = null;
    this.$element.nativeElement.focus();
  }
}
