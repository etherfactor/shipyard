import { ExtendedRoute } from "../../app.routes";
import { authorizationGuard } from "../../shared/guards/authorization/authorization.guard";
import { PermissionId } from "../security/models/permission-id";
import { SecurableType } from "../security/models/securable-type";

export const HOME_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/dashboard/dashboard.component").then(m => m.DashboardComponent),
    title: "Shipyard | Dashboard",
    canActivate: [
      authorizationGuard(SecurableType.Package, PermissionId.Read),
    ],
    data: {
      breadcrumb: {
        label: "Dashboard",
        link: "/",
      },
      parentBreadcrumbs: [],
    },
  },
];
