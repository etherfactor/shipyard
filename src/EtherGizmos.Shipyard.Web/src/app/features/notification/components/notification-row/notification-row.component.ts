import { CommonModule } from "@angular/common";
import { Component, HostBinding, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DateTime } from "luxon";
import { Notification } from "../../models/notification";
import { NotificationEventTheme } from "../../models/notification-theme";

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

  @HostBinding("class.unread")
  get unread() {
    return true;
  }

  get colorClass() {
    const eventId = this.notification.notificationSubscription?.notificationEventId;
    if (!eventId) return "text-secondary";
    return NotificationEventTheme[eventId].colorClass ?? "text-secondary";
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
}
