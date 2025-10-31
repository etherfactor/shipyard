import { ExtendedRoute } from "../../app.routes";

export const CARRIER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-list/carrier-list.component").then(m => m.CarrierListComponent),
    title: "Shipyard | Carrier List",
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
