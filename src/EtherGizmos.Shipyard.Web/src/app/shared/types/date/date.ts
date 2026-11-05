import { DateTime } from "luxon";
import { z } from "zod";

export function isDateString(value: unknown): value is string {
  if (typeof value === 'string') {
    return DateTime.fromISO(value).isValid;
  }

  return false;
}

export function isDate(value: unknown): value is DateTime {
  return DateTime.isDateTime(value);
}

export function parseDate(value: unknown): DateTime {
  if (isDate(value)) {
    return value;
  } else if (isDateString(value)) {
    return DateTime.fromISO(value).startOf("day");
  }

  throw new Error(`Value ${value} is not a valid datetime.`);
}

export const DateZ = z.custom<DateTime>(value => {
  return isDate(value) || isDateString(value);
}).transform(value => {
  return parseDate(value);
});
