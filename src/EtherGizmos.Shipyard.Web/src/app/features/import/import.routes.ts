import { ExtendedRoute } from "../../app.routes";

export const IMPORT_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/import-home/import-home.component").then(m => m.ImportHomeComponent),
    title: "Shipyard | Import",
    canActivate: [],
    data: {
      breadcrumb: {
        label: "Import",
        link: "/import",
      },
      parentBreadcrumbs: [],
    },
  },
];
