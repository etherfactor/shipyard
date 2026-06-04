import { inject, Injectable } from "@angular/core";
import { ODataClient } from "@ethergizmos/odata-fluent-client";
import { narrowValidator, o } from "../../../../shared/utilities/odata/odata.util";
import { Notification, NotificationZ } from "../../models/notification";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<Notification>("notifications")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(NotificationZ, selectExpand);
        const parsed = validator.parse(value);
        return parsed;
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
}
