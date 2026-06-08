import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { Router } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { JsonSchemaAutoFormComponent } from "../../../../shared/components/json-schema-auto-form/json-schema-auto-form.component";
import { ReadonlyFormDirective } from "../../../../shared/directives/readonly-form/readonly-form.directive";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { FilterValue } from "../../../../shared/utilities/filter/filter.util";
import { TypedFormGroup } from "../../../../shared/utilities/form/form.util";
import { Notification } from "../../models/notification";
import { NotificationChannel } from "../../models/notification-channel";
import { NotificationEvent } from "../../models/notification-event";
import { NotificationSchedule } from "../../models/notification-schedule";
import { NotificationSubscription } from "../../models/notification-subscription";
import { NotificationChannelTheme, NotificationEventTheme, NotificationScheduleTheme, NotificationTheme } from "../../models/notification-theme";
import { NotificationMetaService } from "../../services/notification-meta/notification-meta.service";
import { JsonSchemaZ } from "../../../../shared/types/json-schema/json-schema";

console.log(JsonSchemaZ);

@Component({
  selector: "app-notification-subscription-detail",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    JsonSchemaAutoFormComponent,
    NgSelectModule,
    ReadonlyFormDirective,
  ],
  templateUrl: "./notification-subscription-detail.component.html",
  styleUrl: "./notification-subscription-detail.component.scss",
})
export class NotificationSubscriptionDetailComponent implements OnInit {
  private readonly $notificationMeta = inject(NotificationMetaService);
  private readonly $router = inject(Router);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly isEditing$$ = signal(false);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly subscription$$ = signal<NotificationSubscription | undefined>(undefined);
  readonly form$$ = signal<TypedFormGroup<NotificationSubscription> | undefined>(undefined);

  readonly channelSchema$$ = computed(() => JsonSchemaZ.parse(this.subscription$$()?.channelConfig ?? {
    type: "object",
    properties: {
      value: {
        type: "boolean",
      },
    },
  }));

  readonly events$$ = signal<NotificationEvent[]>([]);
  readonly eventEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.events$$().map(item =>
      [this.formatStyleSpan(NotificationEventTheme, item.id) + " " + this.lookupName(this.events$$(), item.id), item.id]
    );
  });

  readonly channels$$ = signal<NotificationChannel[]>([]);
  readonly channelEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.channels$$().map(item =>
      [this.formatStyleSpan(NotificationChannelTheme, item.id) + " " + this.lookupName(this.channels$$(), item.id), item.id]
    );
  });

  readonly schedules$$ = signal<NotificationSchedule[]>([]);
  readonly scheduleEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.schedules$$().map(item =>
      [this.formatStyleSpan(NotificationScheduleTheme, item.id) + " " + this.lookupName(this.schedules$$(), item.id), item.id]
    );
  });

  readonly delivery$$ = signal<Notification | undefined>(undefined);

  readonly deliveryButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    if (!this.isEditing$$()) {
      buttons.push({
        color: "primary",
        text: "View all",
        callback: this.viewDeliveries,
      });
    }

    return buttons;
  });

  ngOnInit() {
    this.loadEvents();
    this.loadChannels();
    this.loadSchedules();
  }

  async loadEvents() {
    await this.doWork(async () => {
      const events = await this.$notificationMeta.events.search()
        .orderBy("id")
        .execute()
        .data;

      this.events$$.set(events);
    });
  }

  async loadChannels() {
    await this.doWork(async () => {
      const channels = await this.$notificationMeta.channels.search()
        .orderBy("id")
        .execute()
        .data;

      this.channels$$.set(channels);
    });
  }

  async loadSchedules() {
    await this.doWork(async () => {
      const schedules = await this.$notificationMeta.schedules.search()
        .orderBy("id")
        .execute()
        .data;

      this.schedules$$.set(schedules);
    });
  }

  @Bound viewDeliveries() {
    this.$router.navigate(["/notifications", "subscriptions", 0, "deliveries"]);
  }

  async doWork(action: () => void | Promise<void>) {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    try {
      const result = action();
      if (result instanceof Promise) {
        await result;
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  formatStyleSpan(themes: Record<string, NotificationTheme>, id: string) {
    const theme = themes[id];
    if (theme) {
      return `<span class="bi ${theme.iconClass} ${theme.colorClass}"></span>`;
    } else {
      return `<span class="bi bi-question-circle text-secondary"></span>`;
    }
  }

  lookupName(names: { id: string, name: string }[], id: string) {
    return names.find(e => e.id === id)?.name ?? id;
  }

  NotificationEventTheme = NotificationEventTheme;
  NotificationChannelTheme = NotificationChannelTheme;
  NotificationScheduleTheme = NotificationScheduleTheme;
}
