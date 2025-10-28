import { HttpClient, HttpResponse } from "@angular/common/http";
import { Provider } from "@angular/core";
import { createOperatorFactory, HttpClientAdapter, ODataClient, Value } from "@ethergizmos/odata-fluent-client";
import { EntitySelectExpand } from "@ethergizmos/odata-fluent-client/dist/src/core/entity/expand/entity-select-expand";
import { DateTime, Interval } from "luxon";
import { firstValueFrom, Observable } from "rxjs";
import z, { ZodArray, ZodDefault, ZodLazy, ZodNullable, ZodObject, ZodOptional, ZodRawShape, ZodType, ZodTypeAny } from "zod";
import { APP_CONFIG, AppConfig } from "../config/config.util";

export const o = createOperatorFactory({
  date: (value: DateTime) => new LuxonDateValue(value),
  dateTime: (value: DateTime) => new LuxonDateTimeValue(value),
  time: (value: Interval) => new LuxonTimeValue(value),
  paramString: (value: string) => new UnwrappedStringValue(value),
});

export function provideODataClient(): Provider {
  return {
    provide: ODataClient,
    useFactory: (config: AppConfig, $http: HttpClient) => {
      const angularAdapter: HttpClientAdapter = {
        async invoke(request) {
          let response: Observable<HttpResponse<string>>;

          switch (request.method) {
            case "GET":
              response = $http.get(
                request.url,
                {
                  headers: request.headers,
                  params: request.query,
                  observe: "response",
                  responseType: "text",
                });
              break;

            case "DELETE":
              response = $http.delete(
                request.url,
                {
                  headers: request.headers,
                  params: request.query,
                  observe: "response",
                  responseType: "text",
                });
              break;

            case "PATCH":
              response = $http.patch(
                request.url,
                request.body,
                {
                  headers: request.headers,
                  params: request.query,
                  observe: "response",
                  responseType: "text",
                });
              break;

            case "POST":
              response = $http.post(
                request.url,
                request.body,
                {
                  headers: request.headers,
                  params: request.query,
                  observe: "response",
                  responseType: "text",
                });
              break;

            case "PUT":
              response = $http.put(
                request.url,
                request.body,
                {
                  headers: request.headers,
                  params: request.query,
                  observe: "response",
                  responseType: "text",
                });
              break;

            default:
              throw new Error("Unrecognized method");
          }

          const result = await firstValueFrom(response);
          let body: unknown = result.body!;

          try {
            body = JSON.parse(body as string);
          } catch { }

          return {
            status: result.status,
            data: Promise.resolve(body),
          };
        }
      };

      return new ODataClient({
        http: {
          adapter: angularAdapter,
        },
        serviceUrl: config.resourceServer,
        routingType: "parentheses",
      });
    },
    deps: [APP_CONFIG, HttpClient]
  };
}

class LuxonDateTimeValue implements Value<DateTime> {

  private readonly value: DateTime;

  _?: DateTime<boolean> | undefined;

  constructor(value: DateTime) {
    this.value = value;
  }

  toString(): string {
    return this.value.toISO()!;
  }

  eval(): DateTime<boolean> {
    return this.value;
  }
}

class LuxonDateValue implements Value<DateTime> {

  private readonly value: DateTime;

  _?: DateTime<boolean> | undefined;

  constructor(value: DateTime) {
    this.value = value;
  }

  toString(): string {
    return this.value.toISODate()!;
  }

  eval(): DateTime<boolean> {
    return this.value.startOf("day");
  }
}

class LuxonTimeValue implements Value<Interval> {

  private readonly value: Interval;

  _?: Interval<boolean> | undefined;

  constructor(value: Interval) {
    this.value = value;
  }

  toString(): string {
    return this.value.toISOTime()!;
  }

  eval(): Interval<boolean> {
    return this.value;
  }
}

class EnumValue<TEnum extends string> implements Value<TEnum> {

  private readonly value: TEnum;

  _?: TEnum | undefined;

  constructor(value: TEnum) {
    this.value = value;
  }

  toString(): string {
    return `'${this.value.replace(/'/g, "''")}'`;
  }

  eval(): TEnum {
    return this.value;
  }
}

class UnwrappedStringValue implements Value<string> {

  private readonly value: string;

  _?: string | undefined;

  constructor(value: string) {
    this.value = value;
  }

  toString(): string {
    return this.value;
  }

  eval(): string {
    return this.value;
  }
}

type ZodWrapper =
  | { kind: "optional" }
  | { kind: "nullable" }
  | { kind: "default", defaultValue: unknown }
  | { kind: "array" };

export function narrowValidator<TEntity>(
  schema: ZodType<TEntity>,
  selectExpand: EntitySelectExpand,
): ZodType<TEntity> {
  const [unwrapped, wrappers] = unwrapZod(schema);
  if (!(unwrapped instanceof ZodObject)) {
    return unwrapped;
  }

  const narrowed = narrowSelect(unwrapped, selectExpand.select);

  const expandedShape: ZodRawShape = Object.entries(selectExpand.expand).reduce<ZodRawShape>((acc, data) => {
    const [key, expand] = data;
    const original = unwrapped.shape[key];
    acc[key] = narrowValidator(original, expand);

    return acc;
  }, {});

  let useShape: ZodTypeAny;
  if (Object.keys(expandedShape).length > 0) {
    useShape = z.object({
      ...narrowed.shape,
      ...expandedShape,
    });
  } else {
    useShape = narrowed;
  }

  const wrapped = wrapZod(useShape, wrappers);
  return wrapped as unknown as ZodType<TEntity>;
}

function narrowSelect(
  schema: ZodObject<any>,
  select: string[],
): ZodObject<any> {
  if (select.length === 0) {
    return schema;
  }

  const newShape: ZodRawShape = select.reduce<ZodRawShape>((acc, key) => {
    if (key in schema.shape) {
      acc[key] = schema.shape[key];
    }

    return acc;
  }, {});

  return z.object(newShape);
}

function unwrapZod(
  schema: ZodTypeAny,
): [ZodTypeAny, ZodWrapper[]] {
  const wrappers: ZodWrapper[] = [];
  let current: ZodTypeAny = schema;

  while (true) {
    if (current instanceof ZodOptional) {
      wrappers.push({ kind: "optional" });
      current = current._def.innerType;
      continue;
    }

    if (current instanceof ZodNullable) {
      wrappers.push({ kind: "nullable" });
      current = current._def.innerType;
      continue;
    }

    if (current instanceof ZodDefault) {
      wrappers.push({ kind: "default", defaultValue: current._def.defaultValue });
      current = current._def.innerType;
      continue;
    }

    if (current instanceof ZodArray) {
      wrappers.push({ kind: "array" });
      current = current._def.type;
      continue;
    }

    if (current instanceof ZodLazy) {
      const result = current._def.getter();
      current = result;
      continue;
    }

    break;
  }

  return [current, wrappers];
}

function wrapZod(
  inner: ZodTypeAny,
  wrappers: ZodWrapper[],
): ZodTypeAny {
  return wrappers.reduce((acc, entry) => {
    switch (entry.kind) {
      case "optional":
        return acc.optional();

      case "nullable":
        return acc.nullable();

      case "default":
        return acc.default(entry.defaultValue);

      case "array":
        return z.array(acc);
    }
  }, inner);
}
