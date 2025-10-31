import { Duration } from "luxon";
import { z } from "zod";

export function isDurationString(value: unknown): value is string {
  if (typeof value === 'string') {
    return Duration.fromISOTime(value).isValid;
  }

  return false;
}

export function isDuration(value: unknown): value is Duration {
  return Duration.isDuration(value);
}

export function parseDuration(value: unknown): Duration {
  if (isDuration(value)) {
    return value;
  } else if (isDurationString(value)) {
    return Duration.fromISOTime(value);
  }

  throw new Error(`Value ${value} is not a valid datetime.`);
}

export const DurationZ = z.custom<Duration>(value => {
  return isDuration(value) || isDurationString(value);
}).transform(value => {
  if (typeof value === 'string') {
    return Duration.fromISOTime(value);
  } else {
    return value;
  }
});
