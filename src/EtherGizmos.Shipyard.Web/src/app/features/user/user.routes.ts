import { ExtendedRoute } from "../../app.routes";

export const USER_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/user-list/user-list.component").then(m => m.UserListComponent),
    title: "Shipyard | User List",
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
