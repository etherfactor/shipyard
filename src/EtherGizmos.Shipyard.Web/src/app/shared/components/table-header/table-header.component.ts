import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit, Optional } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SortDirection } from '@ethergizmos/odata-fluent-client';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxMaskDirective } from 'ngx-mask';
import { Subscription, debounceTime } from 'rxjs';
import { SortTableService } from '../../services/sort-table/sort-table.service';
import { generateGuid } from '../../types/guid/guid';
import { FilterCondition, FilterPropertyOperator, FilterType, defaultOperators, displayText, filterConditionForm, showInput } from '../../utilities/filter/filter.util';
import { DefaultControlTypes, TypedFormGroup } from '../../utilities/form/form.util';
import { InputLuxonDatetimeComponent } from '../input-luxon-datetime/input-luxon-datetime.component';
import { TableComponent } from '../table/table.component';

@Component({
  selector: '[app-table-header]',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputLuxonDatetimeComponent,
    NgbDropdownModule,
    NgSelectModule,
    NgxMaskDirective,
    ReactiveFormsModule,
  ],
  templateUrl: './table-header.component.html',
  styleUrl: './table-header.component.scss',
  host: {
    //'(click)': 'onClick($event)',
    //'(mouseenter)': 'onMouseEnter($event)',
    //'(mouseleave)': 'onMouseLeave($event)',
  }
})
export class TableHeaderComponent<TData extends object> implements OnInit, OnDestroy {

  @Input({ alias: 'app-table-header', required: true }) name!: string;

  @Input() type?: FilterType;

  @Input() sortable: boolean = false;

  @Input() filterable: boolean = false;

  private readonly $form: FormBuilder;
  private readonly $sortTable: SortTableService;
  private readonly table: TableComponent<TData>;

  private id = generateGuid();

  filterForm: TypedFormGroup<FilterCondition, DefaultControlTypes>;
  filterSubscriptions: Subscription[] = [];

  get direction() {
    return this.table.getSortDirection(this.name);
  }

  set direction(value: SortDirection | undefined) {
    if (value) {
      this.table.setSortDirection(this.name, { column: this.name, direction: value });
    } else {
      this.table.setSortDirection(this.name, undefined);
    }
  }

  get filter() {
    return this.table.getFilterCondition(this.name);
  }

  set filter(value: FilterCondition | undefined) {
    if (value) {
      this.table.setFilterCondition(this.name, { ...value, column: this.name, type: this.type! });
    } else {
      this.table.setFilterCondition(this.name, undefined);
    }
  }

  guidPatterns = {
    'X': { pattern: /[0-9A-Fa-f]/ },
    '4': { pattern: /4/ },
    '8': { pattern: /[8-9A-Ba-b]/ }
  };

  constructor(
    $form: FormBuilder,
    $sortTable: SortTableService,
    @Optional() table: TableComponent<TData>,
  ) {
    this.$form = $form;
    this.$sortTable = $sortTable;
    this.table = table;

    this.filterForm = filterConditionForm(this.$form, { operator: undefined!, value: undefined });
  }

  ngOnInit(): void {
    if (this.table) {
      this.id = this.table.id;
    }

    const condition = this.filterForm;
    const operatorSub = condition.controls.operator.valueChanges.subscribe(operator => {
      const value = condition.controls.value;
      value.setValue(undefined);

      if (operator && showInput[operator]) {
        value.setValidators([Validators.required]);
      } else {
        value.setValidators([]);
      }

      value.updateValueAndValidity();
    });

    this.filterSubscriptions.push(operatorSub);

    const valueSub = condition.valueChanges.pipe(
      debounceTime(0),
    ).subscribe(() => {
      this.filter = this.getFilterCondition();
    });

    this.filterSubscriptions.push(valueSub);
  }
  
  ngOnDestroy(): void {
    this.$sortTable.setSortDirection(this.id, undefined);

    for (const subscription of this.filterSubscriptions) {
      subscription.unsubscribe();
    }
  }

  stopPropagation($event: Event) {
    $event.stopPropagation();
  }

  onClick() {
    if (this.direction) {
      if (this.direction === 'asc') {
        this.direction = 'desc';
      } else {
        this.direction = undefined;
      }
    } else {
      this.direction = 'asc';
    }
  }

  onMouseEnter() {
    console.log('enter');
  }

  onMouseLeave() {
    console.log('leave');
  }

  getOperators(): FilterPropertyOperator[] {
    return defaultOperators[this.type ?? 'string'];
  }

  getOperatorDisplayName(operator: FilterPropertyOperator) {
    return displayText[this.type ?? 'string'][operator];
  }

  showOperatorValue() {
    const condition = this.filterForm;
    return showInput[condition?.controls?.operator?.value ?? '' as FilterPropertyOperator] ?? false;
  }

  getFilterCondition() {
    const condition = this.filterForm;
    if (condition.invalid)
      return undefined;

    const value = condition.value;
    if (!value.operator)
      return undefined;

    return value as FilterCondition | undefined;
  }

  setFilterCondition(value: FilterCondition) {
    const condition = this.filterForm;
    condition.setValue(value);
  }
}
