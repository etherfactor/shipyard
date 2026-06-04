import { inject, Injectable } from "@angular/core";
import { ODataClient } from "@ethergizmos/odata-fluent-client";
import { narrowValidator, o } from "../../../../shared/utilities/odata/odata.util";
import { NotificationSubscription, NotificationSubscriptionZ } from "../../models/notification-subscription";

@Injectable({
  providedIn: "root",
})
export class NotificationSubscriptionService {
  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<NotificationSubscription>("notificationSubscriptions")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withCreate("POST")
      .withUpdate("PATCH")
      .withDelete("DELETE")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(NotificationSubscriptionZ, selectExpand);
        return validator.parse(value);
      })
      .build();

    this.$set = set;
  }

  search() {
    return this.$set.set;
  }

  get(id: number) {
    return this.$set.read(id);
  }

  create(record: Partial<NotificationSubscription>) {
    return this.$set.create(record);
  }

  update(id: number, record: Partial<NotificationSubscription>) {
    return this.$set.update(id, record);
  }

  delete(id: number) {
    return this.$set.delete(id);
  }
}
