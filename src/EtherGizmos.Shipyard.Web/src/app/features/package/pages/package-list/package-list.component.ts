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
import { Package } from '../../models/package';
import { getStatusTypeMetadata, StatusType } from '../../models/status-type';
import { PackageService } from '../../services/package/package.service';

@Component({
  selector: 'app-package-list',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    NgbPaginationModule,
    RouterModule,
    TableComponent,
    TableHeaderComponent,
  ],
  templateUrl: './package-list.component.html',
  styleUrl: './package-list.component.scss'
})
export class PackageListComponent extends ListComponent<Package> {

  private readonly $package = inject(PackageService);
  private readonly $router = inject(Router);

  override readonly perPage: number = 10;

  override activeSort: SortColumn = {
    column: "modifiedAt",
    direction: "desc",
  };

  protected override get actions(): NavbarAction[] {
    const actions: NavbarAction[] = [

    ];

    if (!this.isLoading()) {
      //actions.push({
      //  icon: 'bi-layout-three-columns',
      //  label: 'Edit Columns',
      //});
      actions.push({
        icon: 'bi-plus-square',
        label: 'Add',
        callback: this.new,
      });
    }

    return actions;
  }

  protected override get columns(): TableColumn[] {
    const columns: TableColumn[] = [];

    return columns;
  }

  protected override getEntitySet(): EntitySet<Package> {
    return this.$package.search();
  }

  @Bound new() {
    this.$router.navigate(["/packages", "new"]);
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }
}
