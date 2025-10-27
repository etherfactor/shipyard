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
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/package-detail/package-detail.component").then(m => m.PackageDetailComponent),
    data: {
      breadcrumb: {
        label: "New Package",
        link: "/packages/new",
      },
      parentBreadcrumbs: [
        {
          label: "Package List",
          link: "/packages",
        },
      ],
    }
  },
  {
    path: ":packageId",
    pathMatch: "full",
    loadComponent: () => import("./pages/package-detail/package-detail.component").then(m => m.PackageDetailComponent),
    data: {
      breadcrumb: {
        label: "Package #{packageId}",
        link: "/packages/{packageId}",
      },
      parentBreadcrumbs: [
        {
          label: "Package List",
          link: "/packages",
        },
      ],
    },
  },
];
