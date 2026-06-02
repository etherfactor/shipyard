import { Component, computed } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { NotificationRowComponent } from "../../components/notification-row/notification-row.component";

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
  readonly notificationButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "Unread",
      callback: () => { },
    });

    buttons.push({
      color: "outline-primary",
      text: "All",
      callback: () => { },
    });

    return buttons;
  });
}
