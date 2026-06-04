import { Component, HostBinding, Input } from "@angular/core";
import { RouterModule } from "@angular/router";
import { Notification } from "../../models/notification";

@Component({
  selector: "app-notification-row",
  imports: [
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
}
