import { Component, computed, inject, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router, RouterModule } from "@angular/router";
import { EntitySet } from "@ethergizmos/odata-fluent-client";
import { NgbPaginationModule } from "@ng-bootstrap/ng-bootstrap";
import { NgSelectModule } from "@ng-select/ng-select";
import { ListComponent, TableColumn } from "../../../../shared/components/_base/list/list.component";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { o } from "../../../../shared/utilities/odata/odata.util";
import { SortColumn } from "../../../../shared/utilities/sort/sort.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { NotificationRowComponent } from "../../components/notification-row/notification-row.component";
import { Notification } from "../../models/notification";
import { NotificationEvent } from "../../models/notification-event";
import { NotificationChannelTheme, NotificationEventTheme, NotificationScheduleTheme, NotificationTheme } from "../../models/notification-theme";
import { NotificationMetaService } from "../../services/notification-meta/notification-meta.service";
import { NotificationService } from "../../services/notification/notification.service";

@Component({
  selector: "app-notification-inbox",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgbPaginationModule,
    NgSelectModule,
    NotificationRowComponent,
    RouterModule,
  ],
  templateUrl: "./notification-inbox.component.html",
  styleUrl: "./notification-inbox.component.scss",
})
export class NotificationInboxComponent extends ListComponent<Notification> implements OnInit {
  private readonly $notification = inject(NotificationService);
  private readonly $notificationMeta = inject(NotificationMetaService);
  private readonly $router = inject(Router);

  searchContents?: string;
  searchState?: string;
  searchEventId = "__all";

  events: NotificationEvent[] = [];

  notificationInboxButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "Manage subscriptions",
      callback: this.manageSubscriptions,
    });

    return buttons;
  });

  override ngOnInit() {
    super.ngOnInit();

    this.loadEvents();
  }

  async loadEvents() {
    await this.doWork(async () => {
      this.events = await this.$notificationMeta.events
        .search()
        .orderBy("name")
        .execute()
        .data;
    });
  }

  override async search() {
    await super.search();

    const contents = this.searchContents;
    if (contents && contents.trim() !== "") {
      this.records = deepSearch(this.records, contents);
    }
  }

  @Bound manageSubscriptions() {
    this.$router.navigate(["/notifications", "subscriptions"]);
  }

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "createdAt",
    direction: "desc",
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

  protected override getEntitySet(): EntitySet<Notification> {
    let set = this.$notification.search()
      .expand("notificationSubscription");

    const eventId = this.searchEventId;
    if (eventId && eventId !== "__all") {
      set = set.filter(e =>
        o.eq(
          e.prop("notificationSubscription/notificationEventId" as any),
          o.string(eventId),
        ),
      );
    }

    return set;
  }

  @Bound new() {
    this.$router.navigate(["/notifications/subscriptions", "new"]);
  }

  formatStyleSpan(themes: Record<string, NotificationTheme>, id: string) {
    const theme = themes[id];
    if (theme) {
      return `<span class="bi ${theme.iconClass} ${theme.colorClass}"></span>`;
    } else {
      return `<span class="bi bi-question-circle text-secondary"></span>`;
    }
  }

  NotificationEventTheme = NotificationEventTheme;
  NotificationChannelTheme = NotificationChannelTheme;
  NotificationScheduleTheme = NotificationScheduleTheme;
}

function deepSearch<T extends object>(data: T[], find: string) {
  if (!find) return data;
  const terms = find.split(" ").map(term => term.trim()).filter(term => term !== "");
  return data.filter(item => terms.every(term => deepCompare(item, term)));
}

function deepCompare(data: any, find: string): boolean {
  if (data === null || data === undefined) {
    return false;
  }

  const term = find.toLowerCase();

  if (typeof data !== "object") {
    return (data.toString() as string).toLowerCase().includes(term);
  }

  if (Array.isArray(data)) {
    return (data as unknown[]).some(item => deepCompare(item, term));
  }

  return Object.values(data).some(value => deepCompare(value, term));
}
