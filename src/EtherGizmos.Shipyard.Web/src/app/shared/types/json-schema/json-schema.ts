import { z } from "zod";
import { InferArrayType } from "../../utilities/form/form.util";

export enum JsonSchemaType {
  Null = "null",
  Boolean = "boolean",
  Integer = "integer",
  Number = "number",
  String = "string",
  Object = "object",
  Array = "array",
}

function makeType<TType extends JsonSchemaType>(type: TType): z.ZodUnion<[
  z.ZodLiteral<TType>,
  z.ZodArray<z.ZodLiteral<TType>>,
]>;
function makeType<TType extends JsonSchemaType[]>(...types: TType): z.ZodUnion<[
  z.ZodLiteral<Exclude<InferArrayType<TType>, JsonSchemaType.Null>>,
  z.ZodArray<z.ZodLiteral<InferArrayType<TType>>>,
]>;
function makeType(...args: any[]): any {
  return z.literal(args).or(z.literal(args.filter(item => args.length === 1 || item !== JsonSchemaType.Null)).array());
}

const JsonSchemaBaseZ = z.object({
  title: z.string().nullish(),
  description: z.string().nullish(),
});

const nulltype = makeType(JsonSchemaType.Null);
const JsonSchemaNullZ = JsonSchemaBaseZ.extend({
  type: nulltype,
  enum: z.literal([null]).array().optional(),
  default: z.null().nullish(),
});
export type JsonSchemaNull = z.infer<typeof JsonSchemaNullZ>;
export function isNullType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaNullZ> {
  return value.type === JsonSchemaType.Null || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Null));
}

const booleantype = makeType(JsonSchemaType.Boolean, JsonSchemaType.Null);
const JsonSchemaBooleanZ = JsonSchemaBaseZ.extend({
  type: booleantype,
  enum: z.boolean().nullable().array().optional(),
  default: z.boolean().nullish(),
});
export type JsonSchemaBoolean = z.infer<typeof JsonSchemaBooleanZ>;
export function isBooleanType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaBooleanZ> {
  return value.type === JsonSchemaType.Boolean || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Boolean || item === JsonSchemaType.Null));
}

const integertype = makeType(JsonSchemaType.Integer, JsonSchemaType.Null);
const JsonSchemaIntegerZ = JsonSchemaBaseZ.extend({
  type: integertype,
  enum: z.number().int().nullable().array().optional(),
  default: z.number().int().nullish(),
  minimum: z.number().int().optional(),
  maximum: z.number().int().optional(),
});
export type JsonSchemaInteger = z.infer<typeof JsonSchemaIntegerZ>;
export function isIntegerType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaIntegerZ> {
  return value.type === JsonSchemaType.Integer || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Integer || item === JsonSchemaType.Null));
}

const numbertype = makeType(JsonSchemaType.Number, JsonSchemaType.Null);
const JsonSchemaNumberZ = JsonSchemaBaseZ.extend({
  type: numbertype,
  enum: z.number().nullable().array().optional(),
  default: z.number().nullish(),
  minimum: z.number().optional(),
  maximum: z.number().optional(),
});
export type JsonSchemaNumber = z.infer<typeof JsonSchemaNumberZ>;
export function isNumberType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaNumberZ> {
  return value.type === JsonSchemaType.Number || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Number || item === JsonSchemaType.Null));
}

const stringtype = makeType(JsonSchemaType.String, JsonSchemaType.Null);
const JsonSchemaStringZ = JsonSchemaBaseZ.extend({
  type: stringtype,
  enum: z.string().nullable().array().optional(),
  default: z.string().nullish(),
  minLength: z.number().int().optional(),
  maxLength: z.number().int().optional(),
  format: z.literal(["email", "date-time", "uri", "uuid"]).optional(),
});
export type JsonSchemaString = z.infer<typeof JsonSchemaStringZ>;
export function isStringType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaBooleanZ> {
  return value.type === JsonSchemaType.String || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.String || item === JsonSchemaType.Null));
}

const JsonSchemaStructZ = z.union([
  JsonSchemaNullZ,
  JsonSchemaBooleanZ,
  JsonSchemaIntegerZ,
  JsonSchemaNumberZ,
  JsonSchemaStringZ,
]);

const objecttype = makeType(JsonSchemaType.Object, JsonSchemaType.Null);
const JsonSchemaObjectZ = JsonSchemaBaseZ.extend({
  type: objecttype,
  default: z.object().loose().nullish(),
  get properties() { return z.record(z.string(), JsonSchemaZ).optional(); },
  required: z.string().array().optional(),
  get additionalProperties() { return z.optional(JsonSchemaZ); },
});
export type JsonSchemaObject = z.infer<typeof JsonSchemaObjectZ>;
export function isObjectType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaObjectZ> {
  return value.type === JsonSchemaType.Object || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Object || item === JsonSchemaType.Null));
}

const arraytype = makeType(JsonSchemaType.Array, JsonSchemaType.Null);
const JsonSchemaArrayZ = JsonSchemaBaseZ.extend({
  type: arraytype,
  default: z.array(z.any()).nullish(),
  get items() { return z.optional(JsonSchemaZ); },
  minItems: z.number().int().optional(),
  maxItems: z.number().int().optional(),
  uniqueItems: z.boolean().optional(),
});
export type JsonSchemaArray = z.infer<typeof JsonSchemaArrayZ>;
export function isArrayType(value: z.infer<typeof JsonSchemaZ>): value is z.infer<typeof JsonSchemaArrayZ> {
  return value.type === JsonSchemaType.Array || (typeof value.type === "object" && value.type.every(item => item === JsonSchemaType.Array || item === JsonSchemaType.Null));
}

export const JsonSchemaZ = z.union([
  JsonSchemaStructZ,
  JsonSchemaObjectZ,
  JsonSchemaArrayZ,
]);

export type JsonSchema = z.infer<typeof JsonSchemaZ>;
