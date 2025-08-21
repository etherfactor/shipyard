import { ExtendedRoute } from "../../app.routes";

export const LOGIN_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/login/login.component").then(m => m.LoginComponent),
    data: {
      breadcrumb: {
        label: "",
        link: "/login",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "callback",
    pathMatch: "full",
    loadComponent: () => import("./pages/login-callback/login-callback.component").then(m => m.LoginCallbackComponent),
    data: {
      breadcrumb: {
        label: "",
        link: "/login/callback",
      },
      parentBreadcrumbs: [],
    },
  },
];
