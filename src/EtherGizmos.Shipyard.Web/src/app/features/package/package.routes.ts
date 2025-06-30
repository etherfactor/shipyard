import { ExtendedRoute } from "../../app.routes";

export const PACKAGE_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/package-list/package-list.component").then(m => m.PackageListComponent),
    data: {
      breadcrumb: {
        label: "Package List",
        link: "/packages",
      },
      parentBreadcrumbs: [],
    },
  },
];
