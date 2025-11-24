import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { parseGuid } from '@ethergizmos/odata-fluent-client/dist/src/types/guid';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { Guid } from '../../../../shared/types/guid/guid';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { TypedFormGroup, getDirtyFormValues } from '../../../../shared/utilities/form/form.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { Group } from '../../../group/models/group';
import { GroupService } from '../../../group/services/group/group.service';
import { UserSessionService, formatName } from '../../../login/services/user-session/user-session.service';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { Role } from '../../../role/models/role';
import { RoleService } from '../../../role/services/role/role.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';
import { User, UserF, userForm } from '../../models/user';
import { UserService } from '../../services/user/user.service';

@Component({
  selector: 'app-user-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss'
})
export class UserDetailComponent {

  private readonly $user = inject(UserService);
  private readonly $group = inject(GroupService);
  private readonly $role = inject(RoleService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<Guid | undefined>(undefined);
  readonly user$$ = signal<User | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<UserF> | undefined>(undefined);

  readonly groups$$ = signal<Group[]>([]);
  private groupsLoaded = false;

  readonly selectedGroup$$ = computed(() => this.groups$$().find(e => e.id === this.form$$()?.value?.groupId));

  readonly roles$$ = signal<Role[]>([]);
  private rolesLoaded = false;

  readonly newRoleIds$$ = signal<FormControl<number[]>>(this.$form.nonNullable.control<number[]>([]));

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.user$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.User, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.User, PermissionId.Delete);
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
    const userId = this.$route.snapshot.paramMap.get("userId");
    if (userId) {
      const id = parseGuid(userId);
      this.id$$.set(id);

      this.load();
    } else {
      this.user$$.set({} as User);
      this.form$$.set(userForm(this.$form, {} as User));

      this.onEdit();
    }
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  getSelectedRoles() {
    return this.newRoleIds$$().value.map(item => this.roles$$().find(e => e.id === item)!);
  }

  private async load(single?: EntitySingle<User>) {
    this.loadGroups();
    this.loadRoles();

    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$user
      .get(id);

    try {
      const exec = single
        .expand("roles")
        .execute();
      const data = await exec.data;

      this.user$$.set(data);
      this.init();

      if (this.groups$$().length === 0) {
        this.groups$$.set([]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadGroups() {
    if (!this.groupsLoaded) {
      this.groupsLoaded = true;
      const result = this.$group
        .search()
        .execute();

      const data = await result.data;
      data.sort((a, b) => a.name.localeCompare(b.name));

      this.groups$$.set(data);
    }
  }

  private async loadRoles() {
    if (!this.rolesLoaded) {
      this.rolesLoaded = true;
      const result = this.$role
        .search()
        .execute();

      const data = await result.data;
      data.sort((a, b) => a.name.localeCompare(b.name));

      this.roles$$.set(data);
    }
  }

  private init() {
    this.form$$.set(userForm(this.$form, this.user$$()));
    this.newRoleIds$$().setValue(this.user$$()?.roles?.map(e => e.id) ?? []);
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
    this.loadGroups();
    this.loadRoles();
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.user$$();
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
        const single = this.$user.update(record.id, data);
        await this.saveRoles();
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$user.create(data).execute();
        const created = await create.data;
        await this.saveRoles();
        this.$router.navigate(["/users", created.id]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async saveRoles() {
    const id = this.id$$();
    if (!id)
      return;

    const oldRoleIds = this.user$$()?.roles?.map(e => e.id) ?? [];
    const newRoleIds = this.newRoleIds$$().value;

    const toAdd = newRoleIds.filter(id => oldRoleIds.indexOf(id) < 0);
    const toRemove = oldRoleIds.filter(id => newRoleIds.indexOf(id) < 0);

    for (const roleId of toAdd) {
      await this.$user.createRefToRole(id, roleId)
        .execute()
        .result;
    }

    for (const roleId of toRemove) {
      await this.$user.deleteRefToRole(id, roleId)
        .execute()
        .result;
    }
  }

  @Bound onCancel() {
    if (this.id$$()) {
      this.isEditing$$.set(false);
      this.init();
    } else {
      this.$router.navigate(["/users"]);
    }
  }

  formatName = formatName;
}
