import { DateTime } from "luxon";
import { z } from "zod";

export function isDateTimeString(value: unknown): value is string {
  if (typeof value === 'string') {
    return DateTime.fromISO(value).isValid;
  }

  return false;
}

export function isDateTime(value: unknown): value is DateTime {
  return DateTime.isDateTime(value);
}

export function parseDateTime(value: unknown): DateTime {
  if (isDateTime(value)) {
    return value;
  } else if (isDateTimeString(value)) {
    return DateTime.fromISO(value);
  }

  throw new Error(`Value ${value} is not a valid datetime.`);
}

export const DateTimeZ = z.custom<DateTime>(value => {
  return isDateTime(value) || isDateTimeString(value);
}).transform(value => {
  if (typeof value === 'string') {
    return DateTime.fromISO(value);
  } else {
    return value;
  }
});
