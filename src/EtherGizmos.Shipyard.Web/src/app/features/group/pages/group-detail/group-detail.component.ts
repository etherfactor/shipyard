import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup, getDirtyFormValues } from '../../../../shared/utilities/form/form.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { Group, GroupF, groupForm } from '../../../group/models/group';
import { GroupService } from '../../../group/services/group/group.service';
import { UserSessionService, formatName } from '../../../login/services/user-session/user-session.service';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';

@Component({
  selector: 'app-group-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
    RouterModule,
  ],
  templateUrl: './group-detail.component.html',
  styleUrl: './group-detail.component.scss',
})
export class GroupDetailComponent {

  private readonly $group = inject(GroupService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly group$$ = signal<Group | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<GroupF> | undefined>(undefined);

  readonly users$$ = computed(() => (this.group$$()?.users ?? []).sort((a, b) => a.username.localeCompare(b.username)));

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.group$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Group, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.Group, PermissionId.Delete);
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

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const groupId = this.$route.snapshot.paramMap.get("groupId");
    if (groupId) {
      const id = parseInt(groupId);
      this.id$$.set(id);

      this.load();
    } else {
      this.group$$.set({} as Group);
      this.form$$.set(groupForm(this.$form, {} as Group));

      this.onEdit();
    }
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load(single?: EntitySingle<Group>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$group
      .get(id);

    try {
      const exec = single
        .expand("users")
        .execute();
      const data = await exec.data;

      this.group$$.set(data);
      this.init();
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    this.form$$.set(groupForm(this.$form, this.group$$()));
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.group$$();
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
        const single = this.$group.update(record.id, data);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$group.create(data).execute();
        const created = await create.data;
        this.$router.navigate(["/groups", created.id]);
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
      this.$router.navigate(["/groups"]);
    }
  }

  formatName = formatName;
}
