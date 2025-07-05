import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { Package, PackageF, PackageZ } from '../../models/package';

@Injectable({
  providedIn: 'root'
})
export class PackageService {

  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<Package>("packages")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withCreate("POST")
      .withUpdate("PATCH")
      .withDelete("DELETE")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(PackageZ, selectExpand);
        const parsed = validator.parse(value);
        parsed.trackingUpdates?.sort((a, b) => a.occurredAt.toMillis() - b.occurredAt.toMillis());
        return parsed;
      })
      .build();

    const findUpdatedPackages = this.$odata
      .function("findUpdatedPackages")
      .withDefaultMethod()
      .withParameters({})
      .withCollectionResponse<Package>()
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(PackageZ, selectExpand);
        const parsed = validator.parse(value);
        parsed.trackingUpdates?.sort((a, b) => a.occurredAt.toMillis() - b.occurredAt.toMillis());
        return parsed;
      })
      .build();

    const set1 = this.$odata.bind
      .function(set, { findUpdatedPackages });

    this.$set = set1;
  }

  search() {
    return this.$set.set;
  }

  get(id: number) {
    return this.$set.read(id);
  }

  create(record: Partial<PackageF>) {
    return this.$set.create(record);
  }

  update(id: number, record: Partial<PackageF>) {
    return this.$set.update(id, record);
  }

  delete(id: number) {
    return this.$set.delete(id);
  }

  findUpdatedPackages(quantity: number) {
    return this.$set.functions
      .findUpdatedPackages.invoke({})
      .top(quantity);
  }
}
