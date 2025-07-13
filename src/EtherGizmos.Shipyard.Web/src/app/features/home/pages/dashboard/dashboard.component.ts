import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { DateTime, Duration } from 'luxon';
import { DetailBoxButton, DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { PackageZ } from '../../../package/models/package';
import { getStatusTypeMetadata, StatusType } from '../../../package/models/status-type';
import { PackageService } from '../../../package/services/package/package.service';
import { TrackingUpdateService } from '../../../package/services/tracking-update/tracking-update.service';

@Component({
  selector: 'app-dashboard',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    RouterModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  readonly $package = inject(PackageService);
  readonly $trackingUpdate = inject(TrackingUpdateService);
  readonly $router = inject(Router);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  packageButtons: DetailBoxButton[] = [
    {
      color: "primary",
      text: "View all",
      callback: () => this.$router.navigate(["/packages"]),
    },
  ];

  inTransit = 0;
  delivered = 0;
  errors = 0;
  stale = 0;
  nextPolls: { contents?: string, carrier: string, interval: string, icon: string, color: string, packageId: number }[] = [];
  recentUpdates: { message: string, icon: string, color: string, packageId: number }[] = [];

  ngOnInit(): void {
    this.loadInTransit();
    this.loadDelivered();
    this.loadErrors();
    this.loadStale();
    this.loadPolls();
    this.loadUpdates();
  }

  private async loadInTransit() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const result = this.$package.search()
      .filter(e =>
        o.eq(
          e.prop("lastStatusType"),
          o.string(StatusType.InTransit),
        ),
      )
      .select("id")
      .top(0)
      .count()
      .execute();

    try {
      this.inTransit = await result.count;
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadDelivered() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const result = this.$package.search()
      .filter(e =>
        o.and(
          o.ge(
            e.prop("createdAt"),
            o.dateTime(DateTime.now().minus({ days: 30 })),
          ),
          o.eq(
            e.prop("lastStatusType"),
            o.string(StatusType.Delivered),
          ),
        ),
      )
      .select("id")
      .top(0)
      .count()
      .execute();

    try {
      this.delivered = await result.count;
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadErrors() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const result = this.$package.search()
      .filter(e =>
        o.and(
          o.eq(
            e.prop("lastStatusType"),
            o.string(StatusType.Unknown),
          ),
        ),
      )
      .select("id")
      .top(0)
      .count()
      .execute();

    try {
      this.errors = await result.count;
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadStale() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    
    try {
      this.stale = 0;
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadPolls() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const result = this.$package.search()
      .filter(e =>
        o.lt(
          e.prop("nextPollAt"),
          o.dateTime(DateTime.fromISO("2100-01-01")),
        ),
      )
      .orderBy("nextPollAt", "desc")
      .expand("carrier")
      .top(5)
      .execute();

    try {
      const data = await result.data;

      const now = DateTime.now().toMillis();
      this.nextPolls = data.map(item => {
        const duration = Duration.fromMillis(item.nextPollAt.toMillis() - now)
          .shiftTo("hours", "minutes", "seconds");

        const text = duration.toHuman({ unitDisplay: "short" }).split(",")[0];
        return {
          contents: item.contents,
          carrier: item.carrier?.name ?? "Unknown",
          interval: duration.toMillis() > 0
            ? `In ${text}`
            : "Now",
          icon: duration.toMillis() <= 1000 * 60 * 5 ? "bi-hourglass-bottom"
            : duration.toMillis() <= 1000 * 60 * 30 ? "bi-hourglass-split"
              : "bi-hourglass-top",
          color: duration.toMillis() <= 1000 * 60 * 5 ? "text-danger"
            : duration.toMillis() <= 1000 * 60 * 30 ? "text-warning"
              : "text-muted",
          packageId: item.id,
        };
      });
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private async loadUpdates() {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    const result = this.$package.findUpdatedPackages(5)
      .expand("trackingUpdates", b => b
        .orderBy("occurredAt", "desc")
        .top(1)
      )
      .execute();

    try {
      const data = await result.data;

      for (let i = 0; i < data.length; i++) {
        const datum = data[i];
        const validator = narrowValidator(PackageZ, { select: [], expand: { trackingUpdates: { select: [], expand: {} } } });
        const parsed = validator.parse(datum);
        parsed.trackingUpdates?.sort((a, b) => a.occurredAt.toMillis() - b.occurredAt.toMillis());

        data[i] = parsed;
      }

      data.sort((a, b) => -(a.trackingUpdates?.[a.trackingUpdates.length - 1]?.occurredAt ?? DateTime.fromISO("2100-01-01")).toMillis()
        + (b.trackingUpdates?.[b.trackingUpdates.length - 1]?.occurredAt ?? DateTime.fromISO("2100-01-01")).toMillis());

      for (const datum of data) {
        datum.trackingUpdates?.sort((a, b) => a.occurredAt.toMillis() - b.occurredAt.toMillis());
      }

      this.recentUpdates = data.map(item => {
        const update = item.trackingUpdates?.[item.trackingUpdates.length - 1];
        const time = update?.occurredAt?.toLocaleString({ dateStyle: "medium" }) ?? "Unknown";
        const contents = item.contents;
        const location = update?.location;
        const status = update?.statusType ?? StatusType.Unknown; 

        let message: string;
        switch (status) {
          case (StatusType.Delivered):
            if (location) {
              message = `${time} — Delivered at ${location}`;
            } else {
              message = `${time} — Delivered`;
            }
            break;

          case (StatusType.Expired):
            message = `${time} — Expired`;
            break;

          case (StatusType.FailedAttempt):
            if (location) {
              message = `${time} — Failed attempt at ${location}`;
            } else {
              message = `${time} — Failed attempt`;
            }
            break;

          case (StatusType.InTransit):
            if (location) {
              message = `${time} — In transit from ${location}`;
            } else {
              message = `${time} — In transit`;
            }
            break;

          case (StatusType.OutForDelivery):
            if (location) {
              message = `${time} — Out for delivery to ${location}`;
            } else {
              message = `${time} — Out for delivery`;
            }
            break;

          case (StatusType.Returned):
            if (location) {
              message = `${time} — Returned from ${location}`;
            } else {
              message = `${time} — Returned`;
            }
            break;

          case (StatusType.Waiting):
            if (location) {
              message = `${time} — Waiting for pickup from ${location}`;
            } else {
              message = `${time} — Waiting for pickup`;
            }
            break;

          case (StatusType.Unknown):
            message = `${time} — Unknown`;
            break;
        }


        const metadata = getStatusTypeMetadata(status);
        return {
          message: `${message} — ${contents}`,
          icon: metadata.icon,
          color: metadata.color,
          packageId: item.id,
        };
      });
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }
}
