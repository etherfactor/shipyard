import { CommonModule } from "@angular/common";
import { ChangeDetectionStrategy, Component } from "@angular/core";
import { JsonFormsControl } from "@jsonforms/angular";
import { isBooleanControl, RankedTester, rankWith } from "@jsonforms/core";
import { ReadonlyFormDirective } from "../directives/readonly-form/readonly-form.directive";

@Component({
  selector: 'BooleanControlRenderer',
  template: `
    <div class="row mt-3" [hidden]="hidden">
      <label [attr.for]="id" class="col-sm-4 col-form-label">
        {{ label }}
      </label>

      <div class="col-sm-8">
        <input [id]="id"
               type="checkbox"
               (change)="onChange($event)"
               [checked]="isChecked()"
               [disabled]="!isEnabled()" />
      </div>

      @if (description) {
        <div class="form-text">
          {{ description }}
        </div>
      }

      @if (error) {
        <div class="invalid-feedback d-block">
          {{ error }}
        </div>
      }
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    ReadonlyFormDirective,
  ],
})
export class BooleanControlRenderer extends JsonFormsControl {
  isChecked = () => this.data || false;
}

export const booleanControlTester: RankedTester = rankWith(2, isBooleanControl);
