import { ExtendedRoute } from "../../app.routes";

export const HOME_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/dashboard/dashboard.component").then(m => m.DashboardComponent),
    data: {
      breadcrumb: {
        label: "Dashboard",
        link: "/",
      },
      parentBreadcrumbs: [],
    },
  },
];
