import { Component, computed, inject } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { DateTime } from "luxon";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
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
  private readonly $router = inject(Router);

  notificationInboxButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "Manage subscriptions",
      callback: this.manageSubscriptions,
    });

    return buttons;
  });

  notification: Notification = {
    id: 1,
    createdAt: DateTime.now(),
    notificationSubscriptionId: 1,
    payload: {},
  };

  manageSubscriptions() {
    this.$router.navigate(["/notifications", "subscriptions"]);
  }
}
