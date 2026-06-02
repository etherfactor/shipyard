import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DateTime } from "luxon";
import { DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { NotificationRowComponent } from "../../components/notification-row/notification-row.component";
import { Notification } from "../../models/notification";

@Component({
  selector: "app-notification-inbox",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NotificationRowComponent,
    RouterModule,
  ],
  templateUrl: "./notification-inbox.component.html",
  styleUrl: "./notification-inbox.component.scss",
})
export class NotificationInboxComponent {
  notification: Notification = {
    id: 1,
    createdAt: DateTime.now(),
    notificationSubscriptionId: 1,
    payload: {},
  };
}
