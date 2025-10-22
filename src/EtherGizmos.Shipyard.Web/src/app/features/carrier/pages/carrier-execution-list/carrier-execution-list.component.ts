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
import { CarrierExecution } from '../../models/carrier-execution';
import { CarrierExecutionService } from '../../services/carrier-execution/carrier-execution.service';
import { o } from '../../../../shared/utilities/odata/odata.util';

@Component({
  selector: 'app-carrier-execution-list',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './carrier-execution-list.component.html',
  styleUrl: './carrier-execution-list.component.scss'
})
export class CarrierExecutionListComponent extends ListComponent<CarrierExecution> {

  private readonly $carrierExecution = inject(CarrierExecutionService);
  private readonly $router = inject(Router);

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "startedAt",
    direction: "desc",
  };

  protected override get actions(): NavbarAction[] {
    const actions: NavbarAction[] = [

    ];

    if (!this.isLoading()) {
      actions.push({
        icon: 'bi-layout-three-columns',
        label: 'Edit Columns',
      });
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<CarrierExecution> {
    return this.$carrierExecution.search()
      .filter(e =>
        o.eq(
          e.prop("carrierId"),
          o.int(5)
        )
      );
  }
}
