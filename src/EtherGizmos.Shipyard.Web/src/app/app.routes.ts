import { Route } from "@angular/router";
import { NavbarBreadcrumb } from "./features/app/components/navbar-breadcrumb/navbar-breadcrumb.component";

export const APP_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    loadChildren: () => import("./features/home/home.routes.ts").then(m => m.HOME_ROUTES),
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
