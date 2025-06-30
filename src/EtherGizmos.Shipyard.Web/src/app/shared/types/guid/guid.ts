import { v4 as uuidv4, validate } from 'uuid';
import { z } from 'zod';

export type Guid = string & { __guidTag: true };

export function generateGuid(): Guid {
  return uuidv4() as Guid;
}

export function isGuid(value: unknown): value is Guid {
  if (typeof value === 'string') {
    return validate(value);
  }

  return false;
}

export function parseGuid(value: unknown): Guid {
  if (isGuid(value)) {
    return value;
  }

  throw new Error(`Value ${value} is not a valid guid.`);
}

export const GuidZ = z.custom<Guid>(value => {
  return isGuid(value);
});
