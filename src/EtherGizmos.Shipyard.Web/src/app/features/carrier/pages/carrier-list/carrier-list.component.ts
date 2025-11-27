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
import { UserSessionService } from '../../../login/services/user-session/user-session.service';
import { PermissionId } from '../../../security/models/permission-id';
import { SecurableType } from '../../../security/models/securable-type';
import { Carrier } from '../../models/carrier';
import { CarrierService } from '../../services/carrier/carrier.service';

@Component({
  selector: 'app-carrier-list',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './carrier-list.component.html',
  styleUrl: './carrier-list.component.scss'
})
export class CarrierListComponent extends ListComponent<Carrier> {

  private readonly $carrier = inject(CarrierService);
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
      const hasWrite = this.$session.hasCapability(SecurableType.Carrier, PermissionId.Write);
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

  protected override getEntitySet(): EntitySet<Carrier> {
    return this.$carrier.search();
  }

  @Bound new() {
    this.$router.navigate(["/carriers", "new"]);
  }
}
