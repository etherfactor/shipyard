import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { EntitySet } from '@ethergizmos/odata-fluent-client';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { isEqual } from 'moderndash';
import { Subject, debounceTime, filter } from 'rxjs';
import { NavbarAction } from '../../../../features/app/components/navbar-action/navbar-action.component';
import { NavbarActionService } from '../../../services/navbar-action/navbar-action.service';
import { FilterColumnCondition, FilterTypeLabel, evaluateSearch } from '../../../utilities/filter/filter.util';
import { SortColumn } from '../../../utilities/sort/sort.util';

@Component({
  selector: 'app-list',
  template: ''
})
export abstract class ListComponent<TEntity> implements OnInit {

  protected readonly $form = inject(FormBuilder);
  protected readonly $modal = inject(NgbModal);
  protected readonly $navbarAction = inject(NavbarActionService);

  activeSort?: SortColumn;
  private activeFilters: FilterColumnCondition[] = [];

  private searchSubject = new Subject<void>();

  readonly isLoading = computed(() => this.isLoadingStack() > 0);
  private readonly isLoadingStack = signal(0);

  page: number = 1;
  count: number = 0;
  protected records: TEntity[] = [];

  constructor() { }

  ngOnInit(): void {
    this.initialize();

    let currentSort = this.activeSort;
    let currentFilters = this.activeFilters;
    this.searchSubject.pipe(
      filter(() => {
        const sortEqual = isEqual(this.activeSort, currentSort);
        const filtersEqual = isEqual(this.activeFilters, currentFilters);
        if (sortEqual && filtersEqual)
          return false;

        currentSort = this.activeSort;
        currentFilters = this.activeFilters;

        return true;
      }),
      debounceTime(250),
    ).subscribe(() => {
      this.search();
    });
  }

  private initialize() {
    this._activeColumns = [...this.columns];
    this.refresh();
    this.search();
  }

  private refresh() {
    this.updateActions();
  }

  protected async search() {
    const set = evaluateSearch(this.getEntitySet(), this.activeFilters, this.activeSort, 1, this.perPage);

    this.isLoadingStack.set(this.isLoadingStack() + 1);

    try {
      const result = set
        .skip((this.page - 1) * this.perPage)
        .top(this.perPage)
        .count()
        .execute();

      this.records = await result.data;
      this.count = await result.count;
    } finally {
      this.isLoadingStack.set(this.isLoadingStack() - 1);
    }
  }

  abstract get perPage(): number;

  protected abstract get actions(): NavbarAction[];

  private updateActions() {
    this.$navbarAction.setActions(this.actions);
  }

  protected abstract get columns(): TableColumn[];

  private _activeColumns: TableColumn[] = [];
  protected get activeColumns(): TableColumn[] {
    return this._activeColumns;
  }

  protected abstract getEntitySet(): EntitySet<TEntity>;

  onSortChange(sort: SortColumn) {
    this.activeSort = sort;
    this.searchSubject.next();
    console.log('sort', sort);
  }

  onFilterChange(filters: FilterColumnCondition[]) {
    this.activeFilters = filters;
    this.searchSubject.next();
  }

  protected async doWork(action: () => void | Promise<void>) {
    this.isLoadingStack.set(this.isLoadingStack() + 1);
    try {
      const result = action();
      if (result instanceof Promise) {
        await result;
      }
    } finally {
      this.isLoadingStack.set(this.isLoadingStack() - 1);
    }
  }
}

export interface TableColumn {
  name: string;
  displayName: string;
  type: FilterTypeLabel;
}
