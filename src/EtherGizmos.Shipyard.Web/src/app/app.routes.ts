import { Route } from "@angular/router";
import { NavbarBreadcrumb } from "./features/app/components/navbar-breadcrumb/navbar-breadcrumb.component";
import { authenticationGuard } from "./shared/guards/authentication/authentication.guard";

export const APP_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    loadChildren: () => import("./features/home/home.routes").then(m => m.HOME_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "login",
    loadChildren: () => import("./features/login/login.routes").then(m => m.LOGIN_ROUTES),
  },
  {
    path: "logout",
    loadChildren: () => import("./features/login/logout.routes").then(m => m.LOGOUT_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "packages",
    loadChildren: () => import("./features/package/package.routes").then(m => m.PACKAGE_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "carriers",
    loadChildren: () => import("./features/carrier/carrier.routes").then(m => m.CARRIER_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "users",
    loadChildren: () => import("./features/user/user.routes").then(m => m.USER_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "groups",
    loadChildren: () => import("./features/group/group.routes").then(m => m.GROUP_ROUTES),
    canActivate: [authenticationGuard],
  },
  {
    path: "",
    loadChildren: () => import("./features/error/error.routes").then(m => m.ERROR_ROUTES),
  },
];

export type ExtendedRoute = ParentRoute | BreadedRoute;

interface ParentRoute extends Required<Pick<Route, "loadChildren">>, Omit<Route, "loadChildren"> { }

interface BreadedRoute extends Route {
  data: {
    breadcrumb: NavbarBreadcrumb,
    parentBreadcrumbs: NavbarBreadcrumb[],
  },
};
