import { Component, computed, inject, signal } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { EntitySet } from "@ethergizmos/odata-fluent-client";
import { NgbPaginationModule } from "@ng-bootstrap/ng-bootstrap";
import { ListComponent, TableColumn } from "../../../../shared/components/_base/list/list.component";
import { DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { TableHeaderComponent } from "../../../../shared/components/table-header/table-header.component";
import { TableComponent } from "../../../../shared/components/table/table.component";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { FilterValue } from "../../../../shared/utilities/filter/filter.util";
import { SortColumn } from "../../../../shared/utilities/sort/sort.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { UserSessionService } from "../../../login/services/user-session/user-session.service";
import { NotificationChannel } from "../../models/notification-channel";
import { NotificationEvent } from "../../models/notification-event";
import { NotificationSchedule } from "../../models/notification-schedule";
import { NotificationSubscription } from "../../models/notification-subscription";
import { NotificationChannelTheme, NotificationEventTheme, NotificationScheduleTheme, NotificationTheme } from "../../models/notification-theme";
import { NotificationMetaService } from "../../services/notification-meta/notification-meta.service";
import { NotificationSubscriptionService } from "../../services/notification-subscription/notification-subscription.service";

@Component({
  selector: "app-notification-subscription-list",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: "./notification-subscription-list.component.html",
  styleUrl: "./notification-subscription-list.component.scss",
})
export class NotificationSubscriptionListComponent extends ListComponent<NotificationSubscription> {
  private readonly $notificationMeta = inject(NotificationMetaService);
  private readonly $notificationSubscription = inject(NotificationSubscriptionService);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  readonly events$$ = signal<NotificationEvent[]>([]);
  readonly eventEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.events$$().map(item =>
      [this.formatStyleSpan(NotificationEventTheme, item.id, item.name), item.id]
    );
  });

  readonly channels$$ = signal<NotificationChannel[]>([]);
  readonly channelEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.channels$$().map(item =>
      [this.formatStyleSpan(NotificationChannelTheme, item.id, item.name), item.id]
    );
  });

  readonly schedules$$ = signal<NotificationSchedule[]>([]);
  readonly scheduleEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.schedules$$().map(item =>
      [this.formatStyleSpan(NotificationScheduleTheme, item.id, item.name), item.id]
    );
  });

  readonly isActiveEnum$$ = computed<[string, FilterValue][]>(() => {
    return [
      ['<span class="bi bi-check-circle text-success"></span> Yes', true],
      ['<span class="bi bi-x-circle text-danger"></span> No', false],
    ];
  });

  override ngOnInit() {
    super.ngOnInit();

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

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "eventType",
    direction: "asc",
  };

  protected override get actions(): NavbarAction[] {
    const actions: NavbarAction[] = [

    ];

    if (!this.isLoading()) {
      const hasWrite = true;
      //actions.push({
      //  icon: 'bi-layout-three-columns',
      //  label: 'Edit Columns',
      //});
      if (hasWrite) {
        actions.push({
          icon: 'bi-plus-square',
          label: 'Add',
          callback: this.new,
        });
      }
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<NotificationSubscription> {
    return this.$notificationSubscription.search();
  }

  @Bound new() {
    this.$router.navigate(["/notifications/subscriptions", "new"]);
  }

  formatStyleSpan(themes: Record<string, NotificationTheme>, id: string, name: string) {
    const theme = themes[id];
    if (theme) {
      return `<span class="bi ${theme.iconClass} ${theme.colorClass}"></span> ${name}`;
    } else {
      return `<span class="bi bi-question-circle text-secondary"></span> ${name}`;
    }
  }

  NotificationEventTheme = NotificationEventTheme;
  NotificationChannelTheme = NotificationChannelTheme;
  NotificationScheduleTheme = NotificationScheduleTheme;
}
