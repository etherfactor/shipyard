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
  {
    path: "subscriptions",
    loadComponent: () => import("./pages/notification-subscription-list/notification-subscription-list.component").then(m => m.NotificationSubscriptionListComponent),
    data: {
      breadcrumb: {
        label: "Notification Subscriptions",
        link: "/notifications/subscriptions",
      },
      parentBreadcrumbs: [
        {
          label: "Notification Inbox",
          link: "/notifications",
        },
      ],
    },
  },
];
