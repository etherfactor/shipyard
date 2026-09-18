import { ExtendedRoute } from "../../app.routes";

export const UNSUBSCRIBE_ROUTES: ExtendedRoute[] = [
  {
    path: "unsubscribe",
    loadComponent: () => import("./pages/unsubscribe/unsubscribe.component").then(m => m.UnsubscribeComponent),
    data: {
      breadcrumb: {
        label: "Unsubscribe",
        link: "/unsubscribe",
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
    path: "unsubscribed",
    loadComponent: () => import("./pages/unsubscribed/unsubscribed.component").then(m => m.UnsubscribedComponent),
    data: {
      breadcrumb: {
        label: "Unsubscribed",
        link: "/unsubscribed",
      },
      parentBreadcrumbs: [
        {
          label: "Notification History",
          link: "/notifications",
        },
      ],
    },
  },
];
