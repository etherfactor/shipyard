import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const ORDER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/order-list/order-list.component").then(m => m.OrderListComponent),
    title: "Shipyard | Order List",
    canActivate: [
      authorizationGuard(SecurableType.Order, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Order List",
        link: "/orders",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/order-detail/order-detail.component").then(m => m.OrderDetailComponent),
    title: "Shipyard | New Order",
    canActivate: [
      authorizationGuard(SecurableType.Order, PermissionId.Read),
      authorizationGuard(SecurableType.Order, PermissionId.Write),
    ],
    data: {
      breadcrumb: {
        label: "New Order",
        link: "/orders/new",
      },
      parentBreadcrumbs: [
        {
          label: "Order List",
          link: "/orders",
        },
      ],
    }
  },
  {
    path: ":orderId",
    pathMatch: "full",
    loadComponent: () => import("./pages/order-detail/order-detail.component").then(m => m.OrderDetailComponent),
    title: "Shipyard | Order #:orderId",
    canActivate: [
      authorizationGuard(SecurableType.Order, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Order #{orderId}",
        link: "/orders/{orderId}",
      },
      parentBreadcrumbs: [
        {
          label: "Order List",
          link: "/orders",
        },
      ],
    },
  },
];
