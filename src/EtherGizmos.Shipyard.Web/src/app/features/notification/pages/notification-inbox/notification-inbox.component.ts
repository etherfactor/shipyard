import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { NotificationRowComponent } from "../../components/notification-row/notification-row.component";
import { Notification } from "../../models/notification";
import { NotificationEvent } from "../../models/notification-event";
import { NotificationMetaService } from "../../services/notification-meta/notification-meta.service";
import { NotificationService } from "../../services/notification/notification.service";

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
export class NotificationInboxComponent implements OnInit {
  private readonly $notification = inject(NotificationService);
  private readonly $notificationMeta = inject(NotificationMetaService);
  private readonly $router = inject(Router);

  events: NotificationEvent[] = [];
  notifications: Notification[] = [];

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  notificationInboxButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "Manage subscriptions",
      callback: this.manageSubscriptions,
    });

    return buttons;
  });

  ngOnInit() {
    this.loadEvents();
    this.loadNotifications();
  }

  async loadEvents() {
    this.events = await this.$notificationMeta.events
      .search()
      .orderBy("name")
      .execute()
      .data;
  }

  async loadNotifications() {
    this.notifications = await this.$notification
      .search()
      .orderBy("createdAt", "desc")
      .execute()
      .data;
  }

  @Bound manageSubscriptions() {
    this.$router.navigate(["/notifications", "subscriptions"]);
  }
}
