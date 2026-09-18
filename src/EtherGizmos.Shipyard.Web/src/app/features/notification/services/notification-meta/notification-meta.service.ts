import { inject, Injectable } from "@angular/core";
import { ODataClient } from "@ethergizmos/odata-fluent-client";
import { narrowValidator, o } from "../../../../shared/utilities/odata/odata.util";
import { NotificationChannel, NotificationChannelZ } from "../../models/notification-channel";
import { NotificationEvent, NotificationEventZ } from "../../models/notification-event";
import { NotificationSchedule, NotificationScheduleZ } from "../../models/notification-schedule";

@Injectable({
  providedIn: "root",
})
export class NotificationMetaService {
  private readonly $odata = inject(ODataClient);
  private readonly $channel;
  private readonly $schedule;
  private readonly $event;

  constructor() {
    const channel = this.$odata
      .entitySet<NotificationChannel>("notificationChannels")
      .withKey("id")
      .withKeyType(o.string)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(NotificationChannelZ, selectExpand);
        const parsed = validator.parse(value);
        return parsed;
      })
      .build();

    this.$channel = channel;

    const schedule = this.$odata
      .entitySet<NotificationSchedule>("notificationSchedules")
      .withKey("id")
      .withKeyType(o.string)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(NotificationScheduleZ, selectExpand);
        const parsed = validator.parse(value);
        return parsed;
      })
      .build();

    this.$schedule = schedule;

    const event = this.$odata
      .entitySet<NotificationEvent>("notificationEvents")
      .withKey("id")
      .withKeyType(o.string)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(NotificationEventZ, selectExpand);
        const parsed = validator.parse(value);
        return parsed;
      })
      .build();

    this.$event = event;
  }

  readonly channels = {
    search: () => {
      return this.$channel.set;
    },
    get: (id: string) => {
      return this.$channel.read(id);
    },
  };

  readonly schedules = {
    search: () => {
      return this.$schedule.set;
    },
    get: (id: string) => {
      return this.$schedule.read(id);
    },
  };

  readonly events = {
    search: () => {
      return this.$event.set;
    },
    get: (id: string) => {
      return this.$event.read(id);
    },
  };
}
