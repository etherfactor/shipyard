import { FormBuilder } from "@angular/forms";
import { EntitySet, Value } from "@ethergizmos/odata-fluent-client";
import { DateTime } from "luxon";
import { Guid } from "../../types/guid/guid";
import { AppValidators, DefaultControlTypes, formFactoryForModel } from "../form/form.util";
import { o } from "../odata/odata.util";
import { SortColumn } from "../sort/sort.util";

export type FilterValue = string | number | boolean | DateTime | Guid;

export type FilterTypeLabel = "boolean" | "datetime" | "guid" | "number" | "string";

export type FilterOperatorLabel = "between" | "contains" | "ends_with" | "equals" | "false" | "greater"
  | "greater_equals" | "less" | "less_equals" | "not_contains" | "not_equals" | "not_null" | "null" | "starts_with"
  | "true";

export type FilterFlag = "nullable";

export interface FilterCondition {
  operator: FilterOperatorLabel | undefined;
  values: FilterValue[];
}

export interface FilterColumnCondition extends FilterCondition {
  column: string;
  type: FilterTypeLabel;
}

export interface FilterType {
  convert: (val: any) => Value<any>;
}

export interface FilterOperator {
  label: string;
  arguments: number;
  types: FilterOperatorType[];
  requiresFlags: FilterFlag[];
  filter(set: EntitySet<any>, col: string, ...val: Value<any>[]): EntitySet<any>;
}

export interface FilterOperatorType {
  type: FilterTypeLabel;
  labelOverride?: string;
}

export const TYPES: Record<FilterTypeLabel, FilterType> = {
  boolean: {
    convert: val => o.bool(val),
  },

  datetime: {
    convert: val => o.dateTime(val),
  },

  guid: {
    convert: val => o.guid(val),
  },

  number: {
    convert: val => o.double(val),
  },

  string: {
    convert: val => o.string(val),
  },
};

export const OPERATORS: Record<FilterOperatorLabel, FilterOperator> = {
  between: {
    label: "between",
    arguments: 2,
    types: [
      {
        type: "datetime",
      },
      {
        type: "number",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val1, val2) => set.filter(e =>
      o.and(
        o.ge(
          e.prop(col),
          val1
        ),
        o.le(
          e.prop(col),
          val2
        ),
      ),
    ),
  },

  contains: {
    label: "contains",
    arguments: 1,
    types: [
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.contains(
        e.prop(col),
        val
      ),
    ),
  },

  ends_with: {
    label: "ends with",
    arguments: 1,
    types: [
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.endsWith(
        e.prop(col),
        val
      ),
    ),
  },

  equals: {
    label: "is equal to",
    arguments: 1,
    types: [
      {
        type: "boolean",
      },
      {
        type: "datetime",
      },
      {
        type: "guid",
      },
      {
        type: "number",
      },
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.eq(
        e.prop(col),
        val
      ),
    ),
  },

  false: {
    label: "is false",
    arguments: 0,
    types: [
      {
        type: "boolean",
      },
    ],
    requiresFlags: [],
    filter: (set, col) => set.filter(e =>
      o.eq(
        e.prop(col),
        o.bool(false)
      ),
    ),
  },

  greater: {
    label: "is greater than",
    arguments: 1,
    types: [
      {
        type: "number",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.gt(
        e.prop(col),
        val
      ),
    ),
  },

  greater_equals: {
    label: "is at least",
    arguments: 1,
    types: [
      {
        type: "datetime",
        labelOverride: "is after",
      },
      {
        type: "number",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.ge(
        e.prop(col),
        val
      ),
    ),
  },

  less: {
    label: "is less than",
    arguments: 1,
    types: [
      {
        type: "number",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.lt(
        e.prop(col),
        val
      ),
    ),
  },

  less_equals: {
    label: "is at most",
    arguments: 1,
    types: [
      {
        type: "datetime",
        labelOverride: "is before",
      },
      {
        type: "number",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.le(
        e.prop(col),
        val),
    ),
  },

  not_contains: {
    label: "does not contain",
    arguments: 1,
    types: [
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.not(
        o.contains(
          e.prop(col),
          val
        )
      ),
    ),
  },

  not_equals: {
    label: "does not equal",
    arguments: 1,
    types: [
      {
        type: "boolean",
      },
      {
        type: "datetime",
      },
      {
        type: "guid",
      },
      {
        type: "number",
      },
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.ne(
        e.prop(col),
        val
      ),
    ),
  },

  not_null: {
    label: "is not null",
    arguments: 0,
    types: [
      {
        type: "boolean",
      },
      {
        type: "datetime",
      },
      {
        type: "guid",
      },
      {
        type: "number",
      },
      {
        type: "string",
      },
    ],
    requiresFlags: ["nullable"],
    filter: (set, col) => set.filter(e =>
      o.ne(
        e.prop(col),
        o.null()
      ),
    ),
  },

  null: {
    label: "is null",
    arguments: 0,
    types: [
      {
        type: "boolean",
      },
      {
        type: "datetime",
      },
      {
        type: "guid",
      },
      {
        type: "number",
      },
      {
        type: "string",
      },
    ],
    requiresFlags: ["nullable"],
    filter: (set, col) => set.filter(e =>
      o.eq(
        e.prop(col),
        o.null()
      ),
    ),
  },

  starts_with: {
    label: "starts with",
    arguments: 1,
    types: [
      {
        type: "string",
      },
    ],
    requiresFlags: [],
    filter: (set, col, val) => set.filter(e =>
      o.startsWith(
        e.prop(col),
        val
      ),
    ),
  },

  true: {
    label: "is true",
    arguments: 0,
    types: [{ type: "boolean" }],
    requiresFlags: [],
    filter: (set, col) => set.filter(e =>
      o.eq(
        e.prop(col),
        o.bool(true)
      ),
    ),
  },
};

export const filterConditionForm = formFactoryForModel<FilterCondition, DefaultControlTypes>(($form: FormBuilder, model: FilterCondition) => {
  return {
    operator: [model.operator],
    values: $form.nonNullable.array(model.values, AppValidators.minArrayLength(0)),
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
    const type = filter.type;
    const typeDef = TYPES[type];

    const operator = filter.operator!;
    const operatorDef = OPERATORS[operator];

    const values = filter.values.map(item => typeDef.convert(item));
    set = operatorDef.filter(set, filter.column, ...values);
  }

  if (sort) {
    set = set.orderBy(sort.column as keyof TEntity & string, sort.direction);
  }

  set = set
    .top(perPage)
    .skip((page - 1) * perPage);

  return set;
}
