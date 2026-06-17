import { CommonModule } from "@angular/common";
import { Component, HostBinding, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DateTime } from "luxon";
import { Notification } from "../../models/notification";
import { NotificationChannelTheme, NotificationEventTheme, NotificationScheduleTheme, NotificationTheme } from "../../models/notification-theme";

@Component({
  selector: "notification-row",
  imports: [
    CommonModule,
    RouterModule,
  ],
  host: {
    class: "list-group-item",
  },
  templateUrl: "./notification-row.component.html",
  styleUrl: "./notification-row.component.scss",
})
export class NotificationRowComponent {
  @Input({ required: true }) notification!: Notification;

  get colorClass() {
    const eventId = this.notification.notificationSubscription?.notificationEventId;
    if (!eventId) return "text-secondary";
    return NotificationEventTheme[eventId].colorClass ?? "text-secondary";
  }

  get iconClass() {
    const eventId = this.notification.notificationSubscription?.notificationEventId;
    if (!eventId) return "bi-bell";
    return NotificationEventTheme[eventId].iconClass ?? "bi-bell";
  }

  formatStyleSpan(themes: Record<string, NotificationTheme>, id: string, name: string) {
    const theme = themes[id];
    const colorClass = theme?.colorClass?.replace(/^text-(?!bg-)/g, "text-bg-");
    if (theme) {
      return `<span class="badge ${colorClass} align-self-start">
        <span class="bi ${theme.iconClass}"></span>
        ${name}
      </span>`;
    } else {
      return `<span class="badge text-bg-secondary align-self-start">
        <span class="bi bi-question-circle"></span>
        ${name}
      </span>`;
    }
  }

  humanizeTimestamp(date: DateTime) {
    const now = DateTime.local();

    //1. Check if it's today or yesterday using toRelativeCalendar
    //toRelativeCalendar handles localized "today", "yesterday", etc.
    const calendarDay = date.toRelativeCalendar({ base: now });

    //2. Format the time part
    const time = date.toFormat('h:mm a'); // e.g., "5:00 PM"

    //3. Return combined string based on calendar day
    if (calendarDay === "today" || calendarDay === "yesterday") {
      return `${calendarDay} at ${time}`;
    }

    //4. Fallback for older dates (e.g., "Oct 25, 2023 at 5:00 PM")
    return date.toLocaleString(DateTime.DATETIME_MED);
  };

  NotificationEventTheme = NotificationEventTheme;
  NotificationChannelTheme = NotificationChannelTheme;
  NotificationScheduleTheme = NotificationScheduleTheme;
}
