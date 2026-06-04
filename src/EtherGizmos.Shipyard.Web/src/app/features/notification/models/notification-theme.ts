export interface NotificationTheme {
  iconClass: string;
  colorClass: string;
}

export const NotificationChannelTheme: Record<string, NotificationTheme> = {
  "email": {
    iconClass: "bi-envelope",
    colorClass: "text-success",
  },
  "webpush": {
    iconClass: "bi-phone",
    colorClass: "text-info",
  },
  "webhook": {
    iconClass: "bi-globe",
    colorClass: "text-warning",
  },
};

export const NotificationScheduleTheme: Record<string, NotificationTheme> = {
  "immediate": {
    iconClass: "bi-lightning",
    colorClass: "text-warning",
  },
  "digest": {
    iconClass: "bi-clock-history",
    colorClass: "text-info",
  },
};

export const NotificationEventTheme: Record<string, NotificationTheme> = {
  "package.delivered": {
    iconClass: "bi-box-seam",
    colorClass: "text-success",
  },
};
