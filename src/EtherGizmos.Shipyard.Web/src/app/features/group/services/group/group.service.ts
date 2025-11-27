import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { Group, GroupF, GroupZ } from '../../models/group';

@Injectable({
  providedIn: 'root',
})
export class GroupService {

  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<Group>("groups")
      .withKey("id")
      .withKeyType(o.int)
      .withRead("GET")
      .withReadSet("GET")
      .withCreate("POST")
      .withUpdate("PATCH")
      .withDelete("DELETE")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(GroupZ, selectExpand);
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

  create(record: Partial<GroupF>) {
    return this.$set.create(record);
  }

  update(id: number, record: Partial<GroupF>) {
    return this.$set.update(id, record);
  }

  delete(id: number) {
    return this.$set.delete(id);
  }
}
