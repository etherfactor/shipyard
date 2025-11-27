import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const GROUP_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/group-list/group-list.component").then(m => m.GroupListComponent),
    title: "Shipyard | Group List",
    canActivate: [
      authorizationGuard(SecurableType.Group, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Group List",
        link: "/groups",
      },
      parentBreadcrumbs: [],
    },
  },
  {
    path: "new",
    pathMatch: "full",
    loadComponent: () => import("./pages/group-detail/group-detail.component").then(m => m.GroupDetailComponent),
    title: "Shipyard | New Group",
    canActivate: [
      authorizationGuard(SecurableType.Group, PermissionId.Read),
      authorizationGuard(SecurableType.Group, PermissionId.Write),
    ],
    data: {
      breadcrumb: {
        label: "New Group",
        link: "/groups/new",
      },
      parentBreadcrumbs: [
        {
          label: "Group List",
          link: "/groups",
        },
      ],
    }
  },
  {
    path: ":groupId",
    pathMatch: "full",
    loadComponent: () => import("./pages/group-detail/group-detail.component").then(m => m.GroupDetailComponent),
    title: "Shipyard | Group #:groupId",
    canActivate: [
      authorizationGuard(SecurableType.Group, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Group #{groupId}",
        link: "/groups/{groupId}",
      },
      parentBreadcrumbs: [
        {
          label: "Group List",
          link: "/groups",
        },
      ],
    },
  },
];
