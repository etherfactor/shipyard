import { Route } from "@angular/router";

export const APP_ROUTES: ExtendedRoute[] = [

];

export type ExtendedRoute = ParentRoute | BreadedRoute;

interface ParentRoute extends Required<Pick<Route, "loadChildren">>, Omit<Route, "loadChildren"> {

}

interface BreadedRoute extends Route {
  data: {
    breadcrumb: NavbarBreadcrumb,
    parentBreadcrumbs: NavbarBreadcrumb[],
  },
};
