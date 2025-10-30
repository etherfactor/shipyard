import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EntitySet } from '@ethergizmos/odata-fluent-client';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { DateTime, Duration } from 'luxon';
import { ListComponent, TableColumn } from '../../../../shared/components/_base/list/list.component';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { TableHeaderComponent } from '../../../../shared/components/table-header/table-header.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { o } from '../../../../shared/utilities/odata/odata.util';
import { SortColumn } from '../../../../shared/utilities/sort/sort.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { CarrierExecution } from '../../models/carrier-execution';
import { ExecutionStatusType, getExecutionStatusTypeMetadata } from '../../models/execution-status-type';
import { CarrierExecutionService } from '../../services/carrier-execution/carrier-execution.service';

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
  private readonly $activatedRoute = inject(ActivatedRoute);
  private readonly $router = inject(Router);

  protected id!: number;

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "startedAt",
    direction: "desc",
  };

  override ngOnInit() {
    this.id = parseInt(this.$activatedRoute.snapshot.paramMap.get("carrierId")!);

    super.ngOnInit();
  }

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

  protected override getEntitySet(): EntitySet<CarrierExecution> {
    return this.$carrierExecution.search()
      .filter(e =>
        o.and(
          o.eq(
            e.prop("carrierId"),
            o.int(this.id)
          ),
          o.ne(
            e.prop("startedAt"),
            o.null()
          )
        )
      )
      .expand("carrier", e => e
        .select("id", "name")
      );
  }
  
  getExecutionStatusMetadata(statusType: ExecutionStatusType) {
    return getExecutionStatusTypeMetadata(statusType);
  }

  getDiffTime(dateTime1: DateTime | null | undefined, dateTime2: DateTime | null | undefined) {
    if (!dateTime1 || !dateTime2)
      return "—";

    let duration = Duration.fromMillis(dateTime2.toMillis() - dateTime1.toMillis())
      .shiftTo("hours", "minutes", "seconds");

    if (duration.hours === 0) {
      duration = duration.shiftTo("minutes", "seconds");

      if (duration.minutes === 0) {
        duration = duration.shiftTo("seconds");
      }
    }

    return duration.toHuman({ unitDisplay: "short" }).split(",")[0];
  }
}
