import { Component, inject } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { EntitySet } from "@ethergizmos/odata-fluent-client";
import { NgbPaginationModule } from "@ng-bootstrap/ng-bootstrap";
import { ListComponent, TableColumn } from "../../../../shared/components/_base/list/list.component";
import { DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { TableHeaderComponent } from "../../../../shared/components/table-header/table-header.component";
import { TableComponent } from "../../../../shared/components/table/table.component";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { SortColumn } from "../../../../shared/utilities/sort/sort.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { UserSessionService } from "../../../login/services/user-session/user-session.service";
import { PermissionId } from "../../../security/models/permission-id";
import { SecurableType } from "../../../security/models/securable-type";
import { Order } from "../../models/order";
import { OrderService } from "../../services/order/order.service";

@Component({
  selector: "app-order-list",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: "./order-list.component.html",
  styleUrl: "./order-list.component.scss",
})
export class OrderListComponent extends ListComponent<Order> {
  private readonly $order = inject(OrderService);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "modifiedAt",
    direction: "desc",
  };

  protected override get actions(): NavbarAction[] {
    const actions: NavbarAction[] = [

    ];

    if (!this.isLoading()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Order, PermissionId.Write);
      //actions.push({
      //  icon: "bi-layout-three-columns",
      //  label: "Edit Columns",
      //});
      if (hasWrite) {
        actions.push({
          icon: "bi-plus-square",
          label: "Add",
          callback: this.new,
        });
      }
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<Order> {
    return this.$order.search();
  }

  @Bound new() {
    this.$router.navigate(["/orders", "new"]);
  }

  // getStatusMetadata(statusType: StatusType) {
  //   return getStatusTypeMetadata(statusType);
  // }
}
