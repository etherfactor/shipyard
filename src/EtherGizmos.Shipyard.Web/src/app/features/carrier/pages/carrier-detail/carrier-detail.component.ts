import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime, Duration } from 'luxon';
import { DetailBoxButton, DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup, getDirtyFormValues } from '../../../../shared/utilities/form/form.util';
import { o } from '../../../../shared/utilities/odata/odata.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';
import { RunbookStepComponent } from '../../components/runbook-step/runbook-step.component';
import { Carrier, carrierForm } from '../../models/carrier';
import { CarrierExecution } from '../../models/carrier-execution';
import { CarrierRunbookStep, carrierRunbookStepForm } from '../../models/carrier-runbook-step';
import { CarrierStatusRule, carrierStatusRuleForm } from '../../models/carrier-status-rule';
import { ExecutionStatusType, getExecutionStatusTypeMetadata } from '../../models/execution-status-type';
import { CarrierExecutionService } from '../../services/carrier-execution/carrier-execution.service';
import { CarrierService } from '../../services/carrier/carrier.service';

@Component({
  selector: 'app-carrier-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
    RunbookStepComponent,
  ],
  templateUrl: './carrier-detail.component.html',
  styleUrl: './carrier-detail.component.scss'
})
export class CarrierDetailComponent {

  private readonly $carrier = inject(CarrierService);
  private readonly $carrierExecution = inject(CarrierExecutionService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly carrier$$ = signal<Carrier | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<Carrier> | undefined>(undefined);

  readonly carriers$$ = signal<Carrier[]>([]);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly exec$$ = signal<CarrierExecution | undefined>(undefined);

  readonly isLoadingExec$$ = computed(() => this.isLoadingExecStack$$() > 0);
  private readonly isLoadingExecStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    //const record = this.record$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Carrier, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.Carrier, PermissionId.Delete);
      if (!this.isEditing$$()) {
        if (hasWrite) {
          actions.push({
            icon: "bi-pencil",
            label: "Edit",
            callback: this.onEdit,
          });
        }
        if (hasDelete) {
          actions.push({
            icon: "bi-trash",
            label: "Delete",
            callback: this.onDelete,
          });
        }
      } else {
        actions.push({
          icon: "bi-save",
          label: "Save",
          callback: this.onSave,
        });
        actions.push({
          icon: "bi-x-square",
          label: "Cancel",
          callback: this.onCancel,
        });
      }
    }

    return actions;
  });

  readonly ruleButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    if (this.isEditing$$()) {
      buttons.push({
        color: "primary",
        text: "Add rule",
        callback: this.addRule,
      });
    }

    return buttons;
  });

  readonly stepButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    if (this.isEditing$$()) {
      buttons.push({
        color: "primary",
        text: "Add step",
        callback: this.addStep,
      });
    }

    return buttons;
  });

  readonly execButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "View all",
      callback: this.viewExecutions,
    });

    return buttons;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const carrierId = this.$route.snapshot.paramMap.get("carrierId");
    if (carrierId) {
      const id = parseInt(carrierId);
      this.id$$.set(id);

      this.load();
    } else {
      this.carrier$$.set({} as Carrier);
      this.form$$.set(carrierForm(this.$form, {} as Carrier));

      this.onEdit();
    }
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load(single?: EntitySingle<Carrier>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$carrier
      .get(id);

    try {
      const exec = single
        .execute();
      const data = await exec.data;

      data.rules.sort((a, b) => a.priority - b.priority);

      this.carrier$$.set(data);
      this.init();

      try {
        this.isLoadingExecStack$$.set(this.isLoadingExecStack$$() + 1);

        const exec = await this.$carrierExecution.search()
          .filter(e =>
            o.and(
              o.eq(
                e.prop("carrierId"),
                o.int(id)
              ),
              o.ne(
                e.prop("startedAt"),
                o.null()
              )
            )
          )
          .orderBy("startedAt", "desc")
          .execute()
          .data;

        this.exec$$.set(exec[0]);
      } finally {
        this.isLoadingExecStack$$.set(this.isLoadingExecStack$$() - 1);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    this.form$$.set(carrierForm(this.$form, this.carrier$$()));
    if (this.isEditing$$()) {
      this.form$$()?.enable();
    } else {
      this.form$$()?.disable();
    }
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
    this.form$$()?.enable();
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.carrier$$();
    const form = this.form$$();

    if (!record || !form)
      return;

    if (form.invalid) {
      form.markAllAsTouched();
      return;
    }

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    try {
      const data = getDirtyFormValues(form);
      for (const step of data.steps ?? []) {
        this.clean(step);
      }

      if (this.id$$()) {
        const single = this.$carrier.update(record.id, data);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$carrier.create(data).execute();
        const created = await create.data;
        this.$router.navigate(["/carriers", created.id]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  @Bound onCancel() {
    if (this.id$$()) {
      this.isEditing$$.set(false);
      this.init();
    } else {
      this.$router.navigate(["/carriers"]);
    }
  }

  @Bound addRule() {
    const form = this.form$$();
    if (!form)
      return;

    const newForm = carrierStatusRuleForm(this.$form, {} as CarrierStatusRule);
    if (form.disabled) {
      newForm.disable();
    }

    form.controls.rules.push(newForm);
    form.controls.rules.markAsDirty();
  }

  removeRule(index: number) {
    const form = this.form$$();
    if (!form)
      return;

    form.controls.rules.removeAt(index);
    form.controls.rules.markAsDirty();
  }

  @Bound addStep() {
    const form = this.form$$();
    if (!form)
      return;

    const newForm = carrierRunbookStepForm(this.$form, {} as CarrierRunbookStep);
    if (form.disabled) {
      newForm.disable();
    }

    form.controls.steps.push(newForm);
  }

  onMoveUp(index: number) {
    const form = this.form$$();
    if (!form)
      return;

    if (index <= 0)
      return;

    const first = form.controls.steps.controls[index];
    const second = form.controls.steps.controls[index - 1];

    form.controls.steps.controls[index - 1] = first;
    form.controls.steps.controls[index] = second;
  }

  onMoveDown(index: number) {
    const form = this.form$$();
    if (!form)
      return;

    if (index >= form.controls.steps.controls.length - 1)
      return;

    const first = form.controls.steps.controls[index];
    const second = form.controls.steps.controls[index + 1];

    form.controls.steps.controls[index + 1] = first;
    form.controls.steps.controls[index] = second;
  }

  onRemove(index: number) {
    const form = this.form$$();
    if (!form)
      return;

    form.controls.steps.removeAt(index);
  }

  @Bound viewExecutions() {
    this.$router.navigate(["/carriers", this.id$$(), "executions"]);
  }

  private clean(step: CarrierRunbookStep) {
    for (const key of Object.keys(step)) {
      if (step[key as keyof typeof step] === undefined || step[key as keyof typeof step] === null) {
        delete step[key as keyof typeof step];
      }
    }

    for (const subStep of step.steps ?? []) {
      this.clean(subStep);
    }
  }

  getExecutionStatusMetadata(statusType: ExecutionStatusType) {
    return getExecutionStatusTypeMetadata(statusType);
  }

  getDiffTime(dateTime1: DateTime | null | undefined, dateTime2: DateTime | null | undefined) {
    if (!dateTime1 || !dateTime2)
      return "—";

    let duration = Duration.fromMillis(dateTime2.toMillis() - dateTime1.toMillis())
      .shiftTo("hours", "minutes", "seconds");

    if (duration.hours === 0) {
      duration = duration.shiftTo("minutes", "seconds");

      if (duration.minutes === 0) {
        duration = duration.shiftTo("seconds");
      }
    }

    return duration.toHuman({ unitDisplay: "short" }).split(",")[0];
  }

  StatusType = StatusType;
}
