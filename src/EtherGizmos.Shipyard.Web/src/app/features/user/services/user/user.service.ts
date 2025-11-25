import { inject, Injectable } from '@angular/core';
import { ODataClient } from '@ethergizmos/odata-fluent-client';
import { Guid } from '../../../../shared/types/guid/guid';
import { narrowValidator, o } from '../../../../shared/utilities/odata/odata.util';
import { Role } from '../../../role/models/role';
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

    const roles = this.$odata
      .entitySet<Role>("roles")
      .withKey("id")
      .withKeyType(o.int)
      .build();

    const userRoles = this.$odata
      .navigation(set, "roles")
      .withCollection()
      .withReference(roles)
      .withAdd("POST")
      .withRemove("DELETE")
      .build();

    const set2 = this.$odata.bind
      .navigation(set, { roles: userRoles });

    this.$set = set2;
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

  createRefToRole(id: Guid, roleId: number) {
    return this.$set.navigations.roles.add(id, roleId);
  }

  deleteRefToRole(id: Guid, roleId: number) {
    return this.$set.navigations.roles.remove(id, roleId);
  }
}
