import { ExtendedRoute } from "../../app.routes";

export const ERROR_ROUTES: ExtendedRoute[] = [
  {
    path: "403",
    pathMatch: "full",
    loadComponent: () => import("./pages/error-403/error-403.component").then(m => m.Error403Component),
    title: "Shipyard | Forbidden",
    data: {
      breadcrumb: {
        label: "Forbidden",
        link: "/403",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "404",
    pathMatch: "full",
    loadComponent: () => import("./pages/error-404/error-404.component").then(m => m.Error404Component),
    title: "Shipyard | Not Found",
    data: {
      breadcrumb: {
        label: "Not Found",
        link: "/404",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "**",
    redirectTo: "404",
    data: {
      breadcrumb: {
        label: "Not Found",
        link: "/404",
      },
      parentBreadcrumbs: [],
    },
  },
];
