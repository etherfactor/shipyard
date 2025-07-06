import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup, getDirtyFormValues } from '../../../../shared/utilities/form/form.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { Carrier, carrierForm } from '../../models/carrier';
import { CarrierService } from '../../services/carrier/carrier.service';

@Component({
  selector: 'app-carrier-detail',
  imports: [
    CommonModule,
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: './carrier-detail.component.html',
  styleUrl: './carrier-detail.component.scss'
})
export class CarrierDetailComponent {

  private readonly $carrier = inject(CarrierService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly carrier$$ = signal<Carrier | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<Carrier> | undefined>(undefined);

  readonly carriers$$ = signal<Carrier[]>([]);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    //const record = this.record$$();

    if (!this.isLoading$$()) {
      if (!this.isEditing$$()) {
        actions.push({
          icon: "bi-pencil",
          label: "Edit",
          callback: this.onEdit,
        });
        actions.push({
          icon: "bi-trash",
          label: "Delete",
          callback: this.onDelete,
        });
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

      this.carrier$$.set(data);
      this.init();
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    this.form$$.set(carrierForm(this.$form, this.carrier$$()));
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
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
}
