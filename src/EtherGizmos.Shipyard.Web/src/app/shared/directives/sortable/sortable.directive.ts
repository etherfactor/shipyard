import { Directive, ElementRef, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[sortable]',
  standalone: true
})
export class SortableDirective {

  private readonly $element: ElementRef;
  private readonly $renderer2: Renderer2;

  @Input('sortable') column!: string;
  @Input('sorting') sorting?: Sort;

  constructor(
    $element: ElementRef,
    $renderer2: Renderer2,
  ) {
    this.$element = $element;
    this.$renderer2 = $renderer2;
  }
}

export interface Sort {
  name: string;
  direction: 'asc' | 'desc';
}
