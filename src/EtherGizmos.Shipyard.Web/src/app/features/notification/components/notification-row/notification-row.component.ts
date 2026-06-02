import { Component, HostBinding, Input } from "@angular/core";
import { Notification } from "../../models/notification";

@Component({
  selector: "app-notification-row",
  imports: [],
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
}
