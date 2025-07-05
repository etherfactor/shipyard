import { Directive, ElementRef, HostBinding, Input, inject } from '@angular/core';

@Directive({
  selector: '[readonlyForm]',
  standalone: true
})
export class ReadonlyFormDirective {

  @Input() readonlyForm: boolean = false;
  @Input() formSize: "small" | "medium" = "medium";

  $element = inject(ElementRef);

  @HostBinding('class.form-control-sm')
  get cssIsSmallForm(): boolean {
    return this.formSize === "small" && this.isInputElement();
  }

  @HostBinding('class.form-control')
  get cssIsEditable(): boolean {
    return !this.readonlyForm && this.isInputElement();
  }

  @HostBinding('class.form-control-plaintext')
  get cssIsReadonly(): boolean {
    return this.readonlyForm && this.isInputElement();
  }

  @HostBinding('class.small')
  get cssIsSmall(): boolean {
    return this.formSize === "small";
  }

  @HostBinding('attr.readonly')
  get attrIsReadonly(): string | null {
    return this.readonlyForm ? '' : null;
  }

  @HostBinding('attr.plaintext')
  get attrIsPlaintext(): string | null {
    return this.readonlyForm ? '' : null;
  }

  private isInputElement(): boolean {
    return this.$element.nativeElement instanceof HTMLInputElement
      || this.$element.nativeElement instanceof HTMLTextAreaElement;
  }
}
