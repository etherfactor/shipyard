import { ExtendedRoute } from "../../app.routes";

export const NOTIFICATION_ROUTES: ExtendedRoute[] = [
  {
    path: "",
    pathMatch: "full",
    loadComponent: () => import("./pages/notification-history/notification-history.component").then(m => m.NotificationHistoryComponent),
    data: {
      breadcrumb: {
        label: "Notification History",
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
        label: "Subscriptions",
        link: "/notifications/subscriptions",
      },
      parentBreadcrumbs: [
        {
          label: "Notification History",
          link: "/notifications",
        },
      ],
    },
  },
  {
    path: "subscriptions/new",
    loadComponent: () => import("./pages/notification-subscription-detail/notification-subscription-detail.component").then(m => m.NotificationSubscriptionDetailComponent),
    data: {
      breadcrumb: {
        label: "New Subscription",
        link: "/notifications/subscriptions/new",
      },
      parentBreadcrumbs: [
        {
          label: "Notification History",
          link: "/notifications",
        },
        {
          label: "Subscriptions",
          link: "/notifications/subscriptions",
        },
      ],
    },
  },
  {
    path: "subscriptions/:subscriptionId",
    loadComponent: () => import("./pages/notification-subscription-detail/notification-subscription-detail.component").then(m => m.NotificationSubscriptionDetailComponent),
    data: {
      breadcrumb: {
        label: "Subscription #{subscriptionId}",
        link: "/notifications/subscriptions/:subscriptionId",
      },
      parentBreadcrumbs: [
        {
          label: "Notification History",
          link: "/notifications",
        },
        {
          label: "Subscriptions",
          link: "/notifications/subscriptions",
        },
      ],
    },
  },
];
