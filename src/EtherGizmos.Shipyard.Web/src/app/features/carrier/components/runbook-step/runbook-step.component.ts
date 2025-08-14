import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { AppValidators, TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { CarrierRunbookStep, carrierRunbookStepForm, fieldsByStepType } from '../../models/carrier-runbook-step';
import { StepType } from '../../models/step-type';

@Component({
  selector: 'app-runbook-step',
  imports: [
    DetailBoxComponent,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: './runbook-step.component.html',
  styleUrl: './runbook-step.component.scss'
})
export class RunbookStepComponent implements OnInit {

  private readonly $form = inject(FormBuilder);

  @Input({ required: true }) form!: TypedFormGroup<CarrierRunbookStep>;
  @Input({ required: true }) index!: number;
  @Input({ required: true }) total!: number;

  @Output() onMoveUp = new EventEmitter<void>();
  @Output() onMoveDown = new EventEmitter<void>();
  @Output() onRemove = new EventEmitter<void>();

  get type() {
    return this.form.controls.stepType.value!;
  }

  ngOnInit(): void {
    this.onStepTypeChange();
  }

  isFieldVisible(field: string) {
    const type = this.type;
    const fields = fieldsByStepType[type] ?? [];
    if (fields.some(e => e.field === field)) {
      return true;
    }

    return false;
  }

  onStepTypeChange() {
    const type = this.type;
    const fields = fieldsByStepType[type] ?? [];

    for (const key of Object.keys(this.form.controls)) {
      if (fields.some(e => e.field === key && e.required)) {
        this.form.controls[key as keyof typeof this.form.controls].setValidators([AppValidators.required]);
      } else {
        this.form.controls[key as keyof typeof this.form.controls].setValidators([]);
      }
      this.form.controls[key as keyof typeof this.form.controls].updateValueAndValidity();
    }
  }

  addStep() {
    const form = this.form;
    if (!form)
      return;

    const newForm = carrierRunbookStepForm(this.$form, {} as CarrierRunbookStep);
    if (form.disabled) {
      newForm.disable();
    }

    form.controls.steps.push(newForm);
  }

  onMoveUpFn(index: number) {
    if (index <= 0)
      return;

    const first = this.form.controls.steps.controls[index];
    const second = this.form.controls.steps.controls[index - 1];

    this.form.controls.steps.controls[index - 1] = first;
    this.form.controls.steps.controls[index] = second;
  }

  onMoveDownFn(index: number) {
    if (index >= this.form.controls.steps.controls.length - 1)
      return;

    const first = this.form.controls.steps.controls[index];
    const second = this.form.controls.steps.controls[index + 1];

    this.form.controls.steps.controls[index + 1] = first;
    this.form.controls.steps.controls[index] = second;
  }

  onRemoveFn(index: number) {
    this.form.controls.steps.removeAt(index);
  }

  StepType = StepType;
}
