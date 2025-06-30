import { FormBuilder, Validators } from "@angular/forms";
import { EntitySet, Value } from "@ethergizmos/odata-fluent-client";
import { DateTime } from "luxon";
import { Guid } from "../../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel } from "../form/form.util";
import { o } from "../odata/odata.util";
import { SortColumn } from "../sort/sort.util";

export type FilterOperator = 'and' | 'or';

export interface FilterCondition {
  operator: FilterPropertyOperator | undefined;
  value: string | number | boolean | DateTime | Guid | undefined;
}

export interface FilterColumnCondition extends FilterCondition {
  column: string;
  type: FilterType;
}

export type FilterType = 'boolean' | 'datetime' | 'guid' | 'number' | 'string';

export type FilterPropertyOperator = 'equals' | 'not_equals' | 'greater' | 'greater_equals'
  | 'less' | 'less_equals' | 'starts_with' | 'ends_with' | 'contains' | 'not_contains'
  | 'true' | 'false' | 'null' | 'not_null';

export const defaultOperators: { [key in FilterType]: FilterPropertyOperator[] } = {
  boolean: ['true', 'false', 'null', 'not_null'],
  datetime: ['equals', 'not_equals', 'greater_equals', 'less_equals', 'null', 'not_null'],
  guid: ['equals', 'not_equals', 'null', 'not_null'],
  number: ['equals', 'not_equals', 'greater', 'greater_equals', 'less', 'less_equals', 'null', 'not_null'],
  string: ['equals', 'not_equals', 'starts_with', 'ends_with', 'contains', 'not_contains', 'null', 'not_null'],
};

export const showInput: { [key in FilterPropertyOperator]: boolean } = {
  contains: true,
  ends_with: true,
  equals: true,
  false: false,
  greater: true,
  greater_equals: true,
  less: true,
  less_equals: true,
  not_contains: true,
  not_equals: true,
  not_null: false,
  null: false,
  starts_with: true,
  true: false,
};

export const displayText: { [key in FilterType]: { [key in FilterPropertyOperator]: string } } = {
  boolean: {
    contains: '',
    ends_with: '',
    equals: '',
    false: 'is false',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: '',
    not_equals: '',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: 'is true',
  },
  datetime: {
    contains: '',
    ends_with: '',
    equals: 'at',
    false: '',
    greater: '',
    greater_equals: 'after',
    less: '',
    less_equals: 'before',
    not_contains: '',
    not_equals: 'not at',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  guid: {
    contains: '',
    ends_with: '',
    equals: 'equals',
    false: '',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: '',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  number: {
    contains: '',
    ends_with: '',
    equals: 'is equal to',
    false: '',
    greater: 'greater than',
    greater_equals: 'greater than or equal to',
    less: 'less than',
    less_equals: 'less than or equal to',
    not_contains: '',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: '',
    true: '',
  },
  string: {
    contains: 'contains',
    ends_with: 'ends with',
    equals: 'is equal to',
    false: '',
    greater: '',
    greater_equals: '',
    less: '',
    less_equals: '',
    not_contains: 'does not contain',
    not_equals: 'does not equal',
    not_null: 'is not null',
    null: 'is null',
    starts_with: 'starts with',
    true: '',
  },
};

export const filterConditionForm = formFactoryForModel<FilterCondition, DefaultControlTypes>(($form: FormBuilder, model: FilterCondition) => {
  return {
    operator: [model.operator],
    value: [model.value, Validators.required],
  };
});

export function evaluateSearch<TEntity>(
  set: EntitySet<TEntity>,
  filters: FilterColumnCondition[],
  sort: SortColumn | undefined,
  page: number,
  perPage: number,
) {
  for (const filter of filters) {
    let rightValue: Value<any>;

    if (filter.value) {
      switch (filter.type) {
        case 'boolean':
          rightValue = o.bool(filter.value as boolean);
          break;

        case 'datetime':
          rightValue = o.dateTime(filter.value as DateTime);
          break;

        case 'guid':
          rightValue = o.guid(filter.value as Guid);
          break;

        case 'number':
          rightValue = o.int(filter.value as number);
          break;

        case 'string':
          rightValue = o.string(filter.value as string);
          break;

        default:
          throw new Error('Not implemented type');
      }
    }

    switch (filter.operator) {
      case 'contains':
        if (filter.type !== 'string')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
        set = set.filter(b =>
          o.contains(
            b.prop(filter.column as keyof TEntity & string) as Value<string>,
            rightValue,
          ),
        );
        break;

      case 'ends_with':
        if (filter.type !== 'string')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
        set = set.filter(b =>
          o.endsWith(
            b.prop(filter.column as keyof TEntity & string) as Value<string>,
            rightValue,
          ),
        );
        break;

      case 'equals':
        set = set.filter(b =>
          o.eq(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'false':
        if (filter.type !== 'boolean')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'boolean'}`);
        set = set.filter(b =>
          o.eq(
            b.prop(filter.column as keyof TEntity & string) as Value<boolean>,
            o.bool(false),
          ),
        );
        break;

      case 'greater':
        set = set.filter(b =>
          o.gt(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'greater_equals':
        set = set.filter(b =>
          o.ge(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'less':
        set = set.filter(b =>
          o.lt(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'less_equals':
        set = set.filter(b =>
          o.le(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'not_contains':
        if (filter.type !== 'string')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
        set = set.filter(b =>
          o.not(
            o.contains(
              b.prop(filter.column as keyof TEntity & string) as Value<string>,
              rightValue,
            ),
          ),
        );
        break;

      case 'not_equals':
        set = set.filter(b =>
          o.ne(
            b.prop(filter.column as keyof TEntity & string),
            rightValue,
          ),
        );
        break;

      case 'not_null':
        set = set.filter(b =>
          o.ne(
            b.prop(filter.column as keyof TEntity & string),
            o.null(),
          ),
        );
        break;

      case 'null':
        set = set.filter(b =>
          o.eq(
            b.prop(filter.column as keyof TEntity & string),
            o.null(),
          ),
        );
        break;

      case 'starts_with':
        if (filter.type !== 'string')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'string'}`);
        set = set.filter(b =>
          o.startsWith(
            b.prop(filter.column as keyof TEntity & string) as Value<string>,
            rightValue,
          ),
        );
        break;

      case 'true':
        if (filter.type !== 'boolean')
          throw new Error(`Unable to filter on ${filter.column} of type ${filter.type}; requires ${'boolean'}`);
        set = set.filter(b =>
          o.eq(
            b.prop(filter.column as keyof TEntity & string) as Value<boolean>,
            o.bool(true),
          ),
        );
        break;
    }
  }

  if (sort) {
    set = set.orderBy(sort.column as keyof TEntity & string, sort.direction);
  }

  set = set
    .top(perPage)
    .skip((page - 1) * perPage);

  return set;
}
