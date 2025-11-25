import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const USER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/user-list/user-list.component").then(m => m.UserListComponent),
    title: "Shipyard | User List",
    canActivate: [
      authorizationGuard(SecurableType.User, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "User List",
        link: "/users",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/user-detail/user-detail.component").then(m => m.UserDetailComponent),
    title: "Shipyard | New User",
    canActivate: [
      authorizationGuard(SecurableType.User, PermissionId.Read),
      authorizationGuard(SecurableType.User, PermissionId.Write),
      authorizationGuard(SecurableType.Group, PermissionId.Read),
      authorizationGuard(SecurableType.Role, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "New User",
        link: "/users/new",
      },
      parentBreadcrumbs: [
        {
          label: "User List",
          link: "/users",
        },
      ],
    }
  },
  {
    path: ":userId",
    pathMatch: "full",
    loadComponent: () => import("./pages/user-detail/user-detail.component").then(m => m.UserDetailComponent),
    title: "Shipyard | User #:userId",
    canActivate: [
      authorizationGuard(SecurableType.User, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "User #{userId}",
        link: "/users/{userId}",
      },
      parentBreadcrumbs: [
        {
          label: "User List",
          link: "/users",
        },
      ],
    },
  },
];
