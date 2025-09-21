import { ExtendedRoute } from "../../app.routes";

export const LOGOUT_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/logout/logout.component").then(m => m.LogoutComponent),
    data: {
      breadcrumb: {
        label: "",
        link: "/login",
      },
      parentBreadcrumbs: [],
    },
  },
];
