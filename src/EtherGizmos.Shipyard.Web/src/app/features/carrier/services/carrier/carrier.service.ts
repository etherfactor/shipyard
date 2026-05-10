import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { buildUrl } from '@ethergizmos/odata-fluent-client/dist/src/utils/http';
import { firstValueFrom } from 'rxjs';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { Carrier, CarrierZ } from '../../models/carrier';

@Injectable({
  providedIn: 'root'
})
export class CarrierService {
  private readonly config = inject(APP_CONFIG);
  private readonly $http = inject(HttpClient);
  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<Carrier>("carriers")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withCreate("POST")
      .withUpdate("PATCH")
      .withDelete("DELETE")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(CarrierZ, selectExpand);
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

  create(record: Partial<Carrier>) {
    return this.$set.create(record);
  }

  update(id: number, record: Partial<Carrier>) {
    return this.$set.update(id, record);
  }

  delete(id: number) {
    return this.$set.delete(id);
  }

  async export(id: number, contentType = "application/yaml") {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `carriers(${id})`, "export");
    return await firstValueFrom(this.$http.get(url, {
      headers: {
        Accept: contentType,
      },
      responseType: "text",
    }));
  }
}
