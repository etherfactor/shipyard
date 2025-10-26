import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { buildUrl } from '@ethergizmos/odata-fluent-client/dist/src/utils/http';
import { firstValueFrom } from 'rxjs';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { CarrierExecution, CarrierExecutionZ } from '../../models/carrier-execution';

@Injectable({
  providedIn: 'root'
})
export class CarrierExecutionService {

  private readonly $odata = inject(ODataClient);
  private readonly $http = inject(HttpClient);
  private readonly config = inject(APP_CONFIG);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<CarrierExecution>("carrierExecutions")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(CarrierExecutionZ, selectExpand);
        return validator.parse(value);
      })
      .build();

    const readArtifact = this.$odata
      .function(set, "readArtifact")
      .withDefaultMethod()
      .withParameters({ uri: o.paramString })
      .withSingleResponse<string>()
      .build();

    const set2 = this.$odata.bind
      .function(set, { readArtifact });

    this.$set = set2;
  }

  search() {
    return this.$set.set;
  }

  get(id: number) {
    return this.$set.read(id);
  }

  async readTextArtifact(id: number, uri: string): Promise<string> {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `carrierExecutions(${id})`, `readArtifact?uri=${encodeURIComponent(uri)}`);
    const text = await firstValueFrom(this.$http.get(url, { responseType: "text" }));
    return text;
  }

  async readBinaryArtifact(id: number, uri: string): Promise<TypedBuffer> {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `carrierExecutions(${id})`, `readArtifact?uri=${encodeURIComponent(uri)}`);
    const response = await firstValueFrom(this.$http.get(url, { observe: "response", responseType: "arraybuffer" }));
    if (!response.body)
      throw new Error("Did not receive a response body");

    return {
      type: response.headers.get("Content-Type")!,
      buffer: response.body,
    };
  }
}

interface TypedBuffer {
  type: string;
  buffer: ArrayBuffer;
}
