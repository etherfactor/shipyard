import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { Package, PackageZ } from '../../models/package';

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

  create(record: Partial<Package>) {
    return this.$set.create(record);
  }

  update(id: number, record: Partial<Package>) {
    return this.$set.update(id, record);
  }

  delete(id: number) {
    return this.$set.delete(id);
  }
}
