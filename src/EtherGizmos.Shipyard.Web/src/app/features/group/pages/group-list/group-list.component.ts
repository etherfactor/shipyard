import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { EntitySet } from '@ethergizmos/odata-fluent-client';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { Bound } from '../../../../shared/utilities/bound/bound.util';
import { SortColumn } from '../../../../shared/utilities/sort/sort.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { GroupService } from '../../../group/services/group/group.service';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';
import { Group } from '../../models/group';

@Component({
  selector: 'app-group-list',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './group-list.component.html',
  styleUrl: './group-list.component.scss',
})
export class GroupListComponent extends ListComponent<Group> {

  private readonly $group = inject(GroupService);
  private readonly $router = inject(Router);
  private readonly $session = inject(UserSessionService);

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "name",
    direction: "asc",
  };

  protected override get actions(): NavbarAction[] {
    const actions: NavbarAction[] = [

    ];

    if (!this.isLoading()) {
      const hasWrite = this.$session.hasCapability(SecurableType.Group, PermissionId.Write);
      //actions.push({
      //  icon: 'bi-layout-three-columns',
      //  label: 'Edit Columns',
      //});
      if (hasWrite) {
        actions.push({
          icon: 'bi-plus-square',
          label: 'Add',
          callback: this.new,
        });
      }
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<Group> {
    return this.$group.search()
      .expand("users", e => e
        .select("id")
      );
  }

  @Bound new() {
    this.$router.navigate(["/groups", "new"]);
  }
}
