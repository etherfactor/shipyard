import { ExtendedRoute } from "../../app.routes";

export const CARRIER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/carrier-list/carrier-list.component").then(m => m.CarrierListComponent),
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
];
