import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { Guid } from '../../../../shared/types/guid/guid';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { User, UserF, UserZ } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly $odata = inject(ODataClient);
  private readonly $set;

  constructor() {
    const set = this.$odata
      .entitySet<User>("users")
      .withKey("id")
      .withKeyType(o.guid)
      .withRead("GET")
      .withReadSet("GET")
      .withCreate("POST")
      .withUpdate("PATCH")
      .withDelete("DELETE")
      .withValidator((value, selectExpand) => {
        const validator = narrowValidator(UserZ, selectExpand);
        const parsed = validator.parse(value);
        return parsed;
      })
      .build();

    this.$set = set;
  }

  search() {
    return this.$set.set;
  }

  get(id: Guid) {
    return this.$set.read(id);
  }

  create(record: Partial<UserF>) {
    return this.$set.create(record);
  }

  update(id: Guid, record: Partial<UserF>) {
    return this.$set.update(id, record);
  }

  delete(id: Guid) {
    return this.$set.delete(id);
  }
}
