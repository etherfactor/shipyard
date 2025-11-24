import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const ROLE_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/role-list/role-list.component").then(m => m.RoleListComponent),
    title: "Shipyard | Role List",
    canActivate: [
      authorizationGuard(SecurableType.Role, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Role List",
        link: "/roles",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/role-detail/role-detail.component").then(m => m.RoleDetailComponent),
    title: "Shipyard | New Role",
    canActivate: [
      authorizationGuard(SecurableType.Role, PermissionId.Read),
      authorizationGuard(SecurableType.Role, PermissionId.Write),
    ],
    data: {
      breadcrumb: {
        label: "New Role",
        link: "/roles/new",
      },
      parentBreadcrumbs: [
        {
          label: "Role List",
          link: "/roles",
        },
      ],
    }
  },
  {
    path: ":roleId",
    pathMatch: "full",
    loadComponent: () => import("./pages/role-detail/role-detail.component").then(m => m.RoleDetailComponent),
    title: "Shipyard | Role #:roleId",
    canActivate: [
      authorizationGuard(SecurableType.Role, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Role #{roleId}",
        link: "/roles/{roleId}",
      },
      parentBreadcrumbs: [
        {
          label: "Role List",
          link: "/roles",
        },
      ],
    },
  },
];
