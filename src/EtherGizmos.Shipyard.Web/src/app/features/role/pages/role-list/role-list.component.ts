import { SlicePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { EntitySet } from '@ethergizmos/odata-fluent-client';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { SortColumn } from '../../../../shared/utilities/sort/sort.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { Role } from '../../../role/models/role';
import { RoleService } from '../../../role/services/role/role.service';

@Component({
  selector: 'app-role-list',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    SlicePipe,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.scss',
})
export class RoleListComponent extends ListComponent<Role> {

  private readonly $role = inject(RoleService);
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
      //actions.push({
      //  icon: 'bi-layout-three-columns',
      //  label: 'Edit Columns',
      //});
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<Role> {
    return this.$role.search()
      .expand("users", e => e
        .select("id")
      );
  }
}
