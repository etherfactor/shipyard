import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const CARRIER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-list/carrier-list.component").then(m => m.CarrierListComponent),
    title: "Shipyard | Carrier List",
    canActivate: [
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Carrier List",
        link: "/carriers",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-detail/carrier-detail.component").then(m => m.CarrierDetailComponent),
    title: "Shipyard | New Carrier",
    canActivate: [
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
      authorizationGuard(SecurableType.Carrier, PermissionId.Write),
    ],
    data: {
      breadcrumb: {
        label: "New Carrier",
        link: "/carriers/new",
      },
      parentBreadcrumbs: [
        {
          label: "Carrier List",
          link: "/carriers",
        },
      ],
    }
  },
  {
    path: ":carrierId",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-detail/carrier-detail.component").then(m => m.CarrierDetailComponent),
    title: "Shipyard | Carrier #:carrierId",
    canActivate: [
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Carrier #{carrierId}",
        link: "/carriers/{carrierId}",
      },
      parentBreadcrumbs: [
        {
          label: "Carrier List",
          link: "/carriers",
        },
      ],
    },
  },
  {
    path: ":carrierId/executions",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-execution-list/carrier-execution-list.component").then(m => m.CarrierExecutionListComponent),
    canActivate: [
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Execution List",
        link: "/carriers/{carrierId}/executions",
      },
      parentBreadcrumbs: [
        {
          label: "Carrier List",
          link: "/carriers",
        },
        {
          label: "Carrier #{carrierId}",
          link: "/carriers/{carrierId}",
        },
      ],
    },
  },
  {
    path: ":carrierId/executions/:executionId",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-execution-detail/carrier-execution-detail.component").then(m => m.CarrierExecutionDetailComponent),
    canActivate: [
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Execution #{executionId}",
        link: "/carriers/{carrierId}/executions/{executionId}",
      },
      parentBreadcrumbs: [
        {
          label: "Carrier List",
          link: "/carriers",
        },
        {
          label: "Carrier #{carrierId}",
          link: "/carriers/{carrierId}",
        },
        {
          label: "Execution List",
          link: "/carriers/{carrierId}/executions",
        },
      ],
    },
  },
];
