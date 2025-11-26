import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const PACKAGE_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/package-list/package-list.component").then(m => m.PackageListComponent),
    title: "Shipyard | Package List",
    canActivate: [
      authorizationGuard(SecurableType.Package, PermissionId.Read),
    ],
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
    title: "Shipyard | New Package",
    canActivate: [
      authorizationGuard(SecurableType.Package, PermissionId.Read),
      authorizationGuard(SecurableType.Package, PermissionId.Write),
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
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
    title: "Shipyard | Package #:packageId",
    canActivate: [
      authorizationGuard(SecurableType.Package, PermissionId.Read),
    ],
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
  {
    path: ":packageId/executions",
    pathMatch: "full",
    loadComponent: () => import("./pages/package-execution-list/package-execution-list.component").then(m => m.PackageExecutionListComponent),
    canActivate: [
      authorizationGuard(SecurableType.Package, PermissionId.Read),
      authorizationGuard(SecurableType.Carrier, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Execution List",
        link: "/packages/{packageId}/executions",
      },
      parentBreadcrumbs: [
        {
          label: "Package List",
          link: "/packages",
        },
        {
          label: "Package #{packageId}",
          link: "/packages/{packageId}",
        },
      ],
    },
  },
];
