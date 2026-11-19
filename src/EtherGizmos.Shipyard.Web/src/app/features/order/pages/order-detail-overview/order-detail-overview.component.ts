import { CurrencyPipe } from "@angular/common";
import { Component, computed, effect, inject, OnInit, signal } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { EntitySingle } from "@ethergizmos/odata-fluent-client";
import { DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { ReadonlyFormDirective } from "../../../../shared/directives/readonly-form/readonly-form.directive";
import { NavbarActionService } from "../../../../shared/services/navbar-action/navbar-action.service";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { UserSessionService } from "../../../login/services/user-session/user-session.service";
import { PermissionId } from "../../../security/models/permission-id";
import { SecurableType } from "../../../security/models/securable-type";
import { Order } from "../../models/order";
import { OrderFulfillmentStatusType } from "../../models/order-fulfillment-status-type";
import { OrderService } from "../../services/order/order.service";

@Component({
  selector: "app-order-detail-overview",
  imports: [
    CurrencyPipe,
    DetailBoxComponent,
    DetailHeaderComponent,
    ReadonlyFormDirective,
  ],
  templateUrl: "./order-detail-overview.component.html",
  styleUrl: "./order-detail-overview.component.scss",
})
export class OrderDetailOverviewComponent implements OnInit {
  private readonly $order = inject(OrderService);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly order$$ = signal<Order | undefined>(undefined);
  readonly items$$ = computed(() => this.order$$()?.orderItems ?? []);

  private readonly distribution$$ = computed(() => {
    const order = this.order$$();
    if (!order)
      return {};

    return getDistribution(order);
  });

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    const record = this.order$$();

    if (!this.isLoading$$()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Order, PermissionId.Write);
      const hasDelete = this.$session.hasCapability(SecurableType.Order, PermissionId.Delete);

      if (hasWrite) {
        actions.push({
          icon: "bi-pencil",
          label: "Edit",
          callback: () => this.$router.navigate(["/orders", this.id$$()]),
        });
      }
      if (hasDelete) {
        actions.push({
          icon: "bi-trash",
          label: "Delete",
          callback: this.onDelete,
        });
      }
    }

    return actions;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const orderId = this.$route.snapshot.paramMap.get("orderId");
    if (!orderId) {
      this.$router.navigate(["/orders"]);
      return;
    }

    const id = parseInt(orderId);
    this.id$$.set(id);

    this.load();
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
        .expand("orderFulfillments", e => e
          .expand("orderFulfillmentItems")
        )
        .execute();
      const data = await exec.data;

      this.order$$.set(data);
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  protected getDisplayName() {
    const order = this.order$$();
    if (!order || !order.orderNumber)
      return `Order #${this.id$$()}`;

    if (order.source) {
      return `${order.source} #${order.orderNumber}`;
    }

    return `Order #${order.orderNumber}`;
  }

  protected getDistributionFor(itemId: number) {
    return this.distribution$$()[itemId] ??= {
      unallocated: 0,
      unallocatedPercentage: 1,
      pending: 0,
      pendingPercentage: 0,
      shipped: 0,
      shippedPercentage: 0,
      received: 0,
      receivedPercentage: 0,
      missing: 0,
      missingPercentage: 0,
      total: 0,
    };
  }

  protected getDistributionLabelFor(itemId: number) {
    const dist = this.distribution$$()[itemId] ??= {
      unallocated: 0,
      unallocatedPercentage: 1,
      pending: 0,
      pendingPercentage: 0,
      shipped: 0,
      shippedPercentage: 0,
      received: 0,
      receivedPercentage: 0,
      missing: 0,
      missingPercentage: 0,
      total: 0,
    };

    const fragments: string[] = [];
    if (dist.missing > 0) {
      fragments.push(`${dist.missing} missing`);
    }

    if (dist.received > 0) {
      fragments.push(`${dist.received} received`);
    }

    if (dist.shipped > 0) {
      fragments.push(`${dist.shipped} shipped`);
    }

    if (dist.pending > 0) {
      fragments.push(`${dist.pending} pending`);
    }

    if (dist.unallocated > 0) {
      fragments.push(`${dist.unallocated} unassigned`);
    }

    if (fragments.length === 0) {
      return "no items";
    }

    return fragments.join(" · ");
  }

  @Bound onDelete() {

  }
}

interface FulfillmentNumber {
  pending: number;
  shipped: number;
  received: number;
  missing: number;
  total: number;
}

interface FulfillmentDistribution {
  unallocated: number;
  unallocatedPercentage: number;
  pending: number;
  pendingPercentage: number;
  shipped: number;
  shippedPercentage: number;
  received: number;
  receivedPercentage: number;
  missing: number;
  missingPercentage: number;
  total: number;
}

export function getDistribution(order: Order): Record<number, FulfillmentDistribution> {
  const map: Record<number, FulfillmentNumber> = {};

  order.orderFulfillments?.reduce((map, fulfillment) => {
    fulfillment.orderFulfillmentItems?.reduce((map, item) => {
      const dist = map[item.orderItemId] ??= {
        pending: 0,
        shipped: 0,
        received: 0,
        missing: 0,
        total: 0,
      };

      if (fulfillment.orderFulfillmentStatusType === OrderFulfillmentStatusType.Received) {
        dist.received += item.receivedQuantity ?? 0;
        dist.missing += Math.max(item.expectedQuantity - (item.receivedQuantity ?? 0), 0);
      } else if (fulfillment.orderFulfillmentStatusType === OrderFulfillmentStatusType.Shipped) {
        dist.shipped += item.expectedQuantity;
      } else {
        dist.pending += item.expectedQuantity;
      }

      return map;
    }, map);

    return map;
  }, map);

  order.orderItems?.reduce((map, item) => {
    const dist = map[item.id] ??= {
      pending: 0,
      shipped: 0,
      received: 0,
      missing: 0,
      total: 0,
    };

    dist.total += item.quantity;

    return map;
  }, map);

  return Object.fromEntries(
    Object.entries(map).map(pair => {
      const numbers = pair[1];

      const remap: FulfillmentDistribution = {
        unallocated: Math.max(numbers.total - numbers.pending - numbers.shipped - numbers.received - numbers.missing, 0),
        unallocatedPercentage: 0,
        pending: numbers.pending,
        pendingPercentage: 0,
        shipped: numbers.shipped,
        shippedPercentage: 0,
        received: numbers.received,
        receivedPercentage: 0,
        missing: numbers.missing,
        missingPercentage: 0,
        total: numbers.total,
      };

      let setUnallocatedTo1 = false;
      if (numbers.total === 0) {
        setUnallocatedTo1 = true;
        numbers.total += 1;
      }

      remap.unallocatedPercentage = setUnallocatedTo1 ? 1 : remap.unallocated / numbers.total;
      remap.pendingPercentage = numbers.pending / numbers.total;
      remap.shippedPercentage = numbers.shipped / numbers.total;
      remap.receivedPercentage = numbers.received / numbers.total;
      remap.missingPercentage = numbers.missing / numbers.total;

      return [pair[0], remap];
    }),
  );
}
