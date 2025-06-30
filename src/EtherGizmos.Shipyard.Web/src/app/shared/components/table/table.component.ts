import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, OnDestroy, OnInit, Output, QueryList, TemplateRef, ViewChildren } from '@angular/core';
import { SortDirection } from '@ethergizmos/odata-fluent-client';
import { Subject, Subscription, combineLatest, debounceTime } from 'rxjs';
import { IteratePipe } from '../../pipes/iterate/iterate.pipe';
import { generateGuid } from '../../types/guid/guid';
import { FilterColumnCondition, FilterCondition } from '../../utilities/filter/filter.util';
import { SortColumn } from '../../utilities/sort/sort.util';
import { TableHeaderComponent } from '../table-header/table-header.component';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    CommonModule,
    IteratePipe,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent<TData extends object> implements OnInit, OnDestroy {

  @Input({ required: true }) data!: TData[];

  @ContentChild('headers') headers!: TemplateRef<any>;

  @ContentChild('rows') rows!: TemplateRef<any>;

  @ViewChildren(TableHeaderComponent) private headerChildren!: QueryList<TableHeaderComponent<TData>>;

  @Input() isLoading = false;

  @Input() sort?: SortColumn;
  @Output() sortChange = new EventEmitter<SortColumn>();
  private debounceSort = new Subject<SortColumn>();

  @Input() filter: FilterColumnCondition[] = [];
  @Output() filterChange = new EventEmitter<FilterColumnCondition[]>();
  private debounceFilter = new Subject<FilterColumnCondition[]>();
  
  private debounceAll = combineLatest([this.debounceSort, this.debounceFilter]);
  private debounceAllSub?: Subscription;

  @Input() minRows: number = 1;

  id = generateGuid();

  constructor() { }

  ngOnInit(): void {
    this.debounceSort.pipe(
      debounceTime(0),
    ).subscribe(() => {
      this.sortChange.emit(this.sort);
    });

    this.debounceFilter.pipe(
      debounceTime(0),
    ).subscribe(() => {
      this.filterChange.emit(this.filter);
    });
  }

  ngOnDestroy(): void {
  }

  getDefaultHeaders(): string[] {
    if (this.data[0]) {
      const keys = Object.keys(this.data[0]);
      return keys;
    } else {
      return [];
    }
  }

  getSortDirection(column: string): SortDirection | undefined {
    if (this.sort?.column === column) {
      return this.sort.direction;
    } else {
      return undefined;
    }
  }

  setSortDirection(column: string, sorting: SortColumn | undefined) {
    this.sort = sorting;
    this.sortChange.emit(this.sort);
  }

  getFilterCondition(column: string): FilterCondition | undefined {
    const maybeFilter = this.filter?.find(e => e.column === column);
    if (maybeFilter) {
      return maybeFilter;
    } else {
      return undefined;
    }
  }

  setFilterCondition(column: string, filter: FilterColumnCondition | undefined) {
    const maybeIndex = this.filter?.findIndex(e => e.column === column);
    const maybeFilter = maybeIndex >= 0 ? this.filter[maybeIndex] : undefined;

    if (maybeIndex >= 0 && filter) {
      this.filter[maybeIndex] = filter;
    } else if (filter) {
      this.filter.push(filter);
    } else if (maybeIndex >= 0) {
      this.filter.splice(maybeIndex, 1);
    }

    if (maybeFilter !== filter || maybeFilter?.operator !== filter?.operator || maybeFilter?.values?.some((value, i) => value !== filter?.values?.[i])) {
      this.filterChange.emit([...this.filter]);
    }
  }
}
