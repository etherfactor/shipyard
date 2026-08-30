import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { ODataClient } from "@ethergizmos/odata-fluent-client";
import { buildUrl } from "@ethergizmos/odata-fluent-client/dist/src/utils/http";
import { firstValueFrom } from "rxjs";
import { APP_CONFIG } from "../../../../shared/utilities/config/config.util";
import { narrowValidator, o } from "../../../../shared/utilities/odata/odata.util";
import { NotificationSubscription, NotificationSubscriptionZ } from "../../models/notification-subscription";

@Injectable({
  providedIn: "root",
})
export class NotificationSubscriptionService {
  private readonly config = inject(APP_CONFIG);
  private readonly $http = inject(HttpClient);
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

    const unsubscribe = this.$odata
      .action(set, "unsubscribe")
      .withDefaultMethod()
      .withParameters({ key: o.string })
      .withSingleResponse<string>()
      .build();

    const set2 = this.$odata.bind
      .action(set, { unsubscribe });

    this.$set = set2;
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

  async unsubscribe(id: number, key: string) {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `notificationSubscriptions(${id})`, "unsubscribe");
    try {
      await firstValueFrom(this.$http.post(url, {}, {
        params: {
          key: key,
        },
        responseType: "text",
      }));
      return true;
    } catch {
      return false;
    }
  }
}
