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

    this.$set = set;
  }

  search() {
    return this.$set.set;
  }

  get(id: number) {
    return this.$set.read(id);
  }
}
