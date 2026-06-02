import { ExtendedRoute } from "../../app.routes";

export const NOTIFICATION_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/notification-inbox/notification-inbox.component").then(m => m.NotificationInboxComponent),
    data: {
      breadcrumb: {
        label: "Notification Inbox",
        link: "/notifications",
      },
      parentBreadcrumbs: [],
    },
  },
];
