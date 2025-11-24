import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { getDirtyFormValues, TypedFormGroup } from '../../../../shared/utilities/form/form.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { Carrier } from '../../../carrier/models/carrier';
import { CarrierService } from '../../../carrier/services/carrier/carrier.service';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';
import { Package, PackageF, packageForm } from '../../models/package';
import { getStatusTypeMetadata, StatusType } from '../../models/status-type';
import { PackageService } from '../../services/package/package.service';

@Component({
  selector: 'app-package-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: './package-detail.component.html',
  styleUrl: './package-detail.component.scss'
})
export class PackageDetailComponent implements OnInit {

  private readonly $package = inject(PackageService);
  private readonly $carrier = inject(CarrierService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly package$$ = signal<Package | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<PackageF> | undefined>(undefined);

  readonly carriers$$ = signal<Carrier[]>([]);
  private carriersLoaded = false;

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.package$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Package, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.Package, PermissionId.Delete);
      if (!this.isEditing$$()) {
        if (this.id$$() && !record?.isDelivered && hasWrite) {
          actions.push({
            icon: "bi-arrow-repeat",
            label: "Repoll",
            callback: this.onRepoll,
          });
        }
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

  get trackingUpdates() {
    const record = this.package$$();
    if (!record)
      return [];

    const updates = record.trackingUpdates ?? [];
    return [...updates].reverse();
  }

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const packageId = this.$route.snapshot.paramMap.get("packageId");
    if (packageId) {
      const id = parseInt(packageId);
      this.id$$.set(id);

      this.load();
    } else {
      this.package$$.set({ lastStatusType: StatusType.Unknown } as Package);
      this.form$$.set(packageForm(this.$form, {} as Package));

      this.onEdit();
    }
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load(single?: EntitySingle<Package>) {
    this.loadCarriers();

    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$package
      .get(id);

    try {
      const exec = single
        .expand("carrier", e =>
          e.select("id", "name")
        )
        .expand("trackingUpdates")
        .execute();
      const data = await exec.data;

      this.package$$.set(data);
      this.init();

      if (this.carriers$$().length === 0) {
        this.carriers$$.set([data.carrier!]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadCarriers() {
    if (!this.carriersLoaded) {
      this.carriersLoaded = true;
      const result = this.$carrier
        .search()
        .execute();

      const data = await result.data;
      data.sort((a, b) => a.name.localeCompare(b.name));

      this.carriers$$.set(data);
    }
  }

  private init() {
    this.form$$.set(packageForm(this.$form, this.package$$()));
  }

  @Bound async onRepoll() {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const poll = this.$package.schedulePoll(id)
      .execute();

    await poll.result;

    this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
    this.loadCarriers();
  }

  @Bound onDelete() {
    
  }

  @Bound async onSave() {
    const record = this.package$$();
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
        const single = this.$package.update(record.id, data);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$package.create(data).execute();
        const created = await create.data;
        this.$router.navigate(["/packages", created.id]);
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
      this.$router.navigate(["/packages"]);
    }
  }
}
