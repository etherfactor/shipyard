import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { TrackingUpdate, TrackingUpdateZ } from '../../models/tracking-update';

@Injectable({
  providedIn: 'root'
})
export class TrackingUpdateService {

  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<TrackingUpdate>("trackingUpdates")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(TrackingUpdateZ, selectExpand);
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
