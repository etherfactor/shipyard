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
import { UserSessionService, formatName } from '../../../login/services/user-session/user-session.service';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { Role, RoleF, roleForm } from '../../models/role';
import { RoleService } from '../../services/role/role.service';

@Component({
  selector: 'app-role-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
    RouterModule,
  ],
  templateUrl: './role-detail.component.html',
  styleUrl: './role-detail.component.scss',
})
export class RoleDetailComponent {

  private readonly $role = inject(RoleService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly role$$ = signal<Role | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<RoleF> | undefined>(undefined);

  readonly users$$ = computed(() => (this.role$$()?.users ?? []).sort((a, b) => a.username.localeCompare(b.username)));

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.role$$();

    if (!this.isLoading$$()) {
      if (!this.isEditing$$()) {
      } else {
      }
    }

    return actions;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const roleId = this.$route.snapshot.paramMap.get("roleId");
    if (roleId) {
      const id = parseInt(roleId);
      this.id$$.set(id);

      this.load();
    } else {
      this.role$$.set({} as Role);
      this.form$$.set(roleForm(this.$form, {} as Role));

      this.onEdit();
    }
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load(single?: EntitySingle<Role>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$role
      .get(id);

    try {
      const exec = single
        .expand("users")
        .execute();
      const data = await exec.data;

      this.role$$.set(data);
      this.init();
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    this.form$$.set(roleForm(this.$form, this.role$$()));
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.role$$();
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
        const single = this.$role.update(record.id, data);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$role.create(data).execute();
        const created = await create.data;
        this.$router.navigate(["/roles", created.id]);
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
      this.$router.navigate(["/roles"]);
    }
  }

  formatName = formatName;
}
