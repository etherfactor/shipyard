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
  if (args.length === 1) {
    args = args.filter(item => item !== JsonSchemaType.Null);
  }
  return z.literal(args).or(z.literal(args).array());
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

const booleantype = makeType(JsonSchemaType.Boolean, JsonSchemaType.Null);
const JsonSchemaBooleanZ = JsonSchemaBaseZ.extend({
  type: booleantype,
  enum: z.boolean().nullable().array().optional(),
  default: z.boolean().nullish(),
});

const integertype = makeType(JsonSchemaType.Integer, JsonSchemaType.Null);
const JsonSchemaIntegerZ = JsonSchemaBaseZ.extend({
  type: integertype,
  enum: z.number().int().nullable().array().optional(),
  default: z.number().int().nullish(),
  minimum: z.number().int().optional(),
  maximum: z.number().int().optional(),
});

const numbertype = makeType(JsonSchemaType.Number, JsonSchemaType.Null);
const JsonSchemaNumberZ = JsonSchemaBaseZ.extend({
  type: numbertype,
  enum: z.number().nullable().array().optional(),
  default: z.number().nullish(),
  minimum: z.number().optional(),
  maximum: z.number().optional(),
});

const stringtype = makeType(JsonSchemaType.String, JsonSchemaType.Null);
const JsonSchemaStringZ = JsonSchemaBaseZ.extend({
  type: stringtype,
  enum: z.string().nullable().array().optional(),
  default: z.string().nullish(),
  minLength: z.number().int().optional(),
  maxLength: z.number().int().optional(),
  format: z.literal(["email", "date-time", "uri", "uuid"]).optional(),
});

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
  properties: z.record(z.string(), JsonSchemaStructZ).optional(),
  required: z.string().array().optional(),
  additionalProperties: z.boolean().optional(),
});

const arraytype = makeType(JsonSchemaType.Array, JsonSchemaType.Null);
const JsonSchemaArrayZ = JsonSchemaBaseZ.extend({
  type: arraytype,
  default: z.array(z.any()).nullish(),
  items: JsonSchemaStructZ.optional(),
  minItems: z.number().int().optional(),
  maxItems: z.number().int().optional(),
  uniqueItems: z.boolean().optional(),
});

export const JsonSchemaZ = z.union([
  JsonSchemaStructZ,
  JsonSchemaObjectZ,
  JsonSchemaArrayZ,
]);

export type JsonSchema = z.infer<typeof JsonSchemaZ>;
