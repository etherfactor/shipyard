import { Component, computed, effect, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { EntityResponse, EntitySingle } from "@ethergizmos/odata-fluent-client";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { InputLuxonDateComponent } from "../../../../shared/components/input-luxon-date/input-luxon-date.component";
import { ReadonlyFormDirective } from "../../../../shared/directives/readonly-form/readonly-form.directive";
import { NavbarActionService } from "../../../../shared/services/navbar-action/navbar-action.service";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { TypedFormGroup, getDirtyFormValues } from "../../../../shared/utilities/form/form.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { UserSessionService } from "../../../login/services/user-session/user-session.service";
import { PermissionId } from "../../../security/models/permission-id";
import { SecurableType } from "../../../security/models/securable-type";
import { Order, OrderF, orderForm } from "../../models/order";
import { OrderItem, OrderItemF, orderItemForm } from "../../models/order-item";
import { OrderItemService } from "../../services/order-item/order-item.service";
import { OrderService } from "../../services/order/order.service";

@Component({
  selector: "app-order-detail",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    InputLuxonDateComponent,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: "./order-detail.component.html",
  styleUrl: "./order-detail.component.scss",
})
export class OrderDetailComponent {
  private readonly $order = inject(OrderService);
  private readonly $orderItem = inject(OrderItemService);
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly order$$ = signal<Order | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<OrderF> | undefined>(undefined);

  itemForms: TypedFormGroup<OrderItemF>[] = [];

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.order$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Order, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.Order, PermissionId.Delete);
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

  readonly itemButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    if (this.isEditing$$()) {
      buttons.push({
        color: "primary",
        text: "Add item",
        callback: this.addItem,
      });
    }

    return buttons;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const orderId = this.$route.snapshot.paramMap.get("orderId");
    if (orderId) {
      const id = parseInt(orderId);
      this.id$$.set(id);

      this.load();
    } else {
      this.order$$.set({} as Order);
      this.form$$.set(orderForm(this.$form, {} as Order));

      this.onEdit();
    }
  }

  private async load(single?: EntitySingle<Order>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$order
      .get(id);

    try {
      const exec = single
        .expand("orderItems")
        .execute();
      const data = await exec.data;

      this.order$$.set(data);
      this.itemForms = data.orderItems!.map(item => orderItemForm(this.$form, item));
      this.init();
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    this.form$$.set(orderForm(this.$form, this.order$$()));
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.order$$();
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
        const single = this.$order.update(record.id, data);
        await this.saveItems(record.id);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$order.create(data).execute();
        const created = await create.data;
        await this.saveItems(created.id);
        this.id$$.set(created.id);
        this.$router.navigate(["/users", created.id]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  async saveItems(orderId: number) {
    for (const itemForm of this.itemForms) {
      const id = itemForm.value.id;
      const item = getDirtyFormValues(itemForm);
      item.orderId = orderId;
      let response: EntityResponse<OrderItem>;
      if (id) {
        response = this.$orderItem.update(id, item).execute();
      } else {
        response = this.$orderItem.create(item).execute();
      }

      await response.result;
    }
  }

  @Bound onCancel() {
    if (this.id$$()) {
      this.isEditing$$.set(false);
      this.init();
    } else {
      this.$router.navigate(["/orders"]);
    }
  }

  @Bound addItem() {
    const form = this.form$$();
    if (!form)
      return;

    const newForm = orderItemForm(this.$form, { orderId: 0 } as OrderItem);
    if (form.disabled) {
      newForm.disable();
    }

    this.itemForms.push(newForm);
    newForm.markAsDirty();
  }

  removeItem(index: number) {
    this.itemForms.splice(index, 1);
  }

  calcLineTotalFor(index: number) {
    const line = this.itemForms[index].value;

    if (line.quantity !== undefined && line.quantity !== null
      && line.unitPrice !== undefined && line.unitPrice !== null) {
      return line.quantity * line.unitPrice;
    } else {
      return null;
    }
  }
}
