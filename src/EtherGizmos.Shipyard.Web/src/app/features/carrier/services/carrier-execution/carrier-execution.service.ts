import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { CarrierExecution, CarrierExecutionZ } from '../../models/carrier-execution';

@Injectable({
  providedIn: 'root'
})
export class CarrierExecutionService {

  private readonly $odata = inject(ODataClient);
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

  readArtifact(id: number, uri: string) {
    return this.$set.functions.readArtifact.invoke(id, { uri });
  }
}
