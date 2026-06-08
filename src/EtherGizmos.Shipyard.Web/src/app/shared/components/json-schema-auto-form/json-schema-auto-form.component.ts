import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { JsonFormsModule } from "@jsonforms/angular";
import { JsonSchema } from "../../types/json-schema/json-schema";

@Component({
  selector: "json-schema-auto-form",
  imports: [
    JsonFormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: "./json-schema-auto-form.component.html",
  styleUrl: "./json-schema-auto-form.component.scss",
})
export class JsonSchemaAutoFormComponent implements OnChanges {
  @Input({ required: true }) schema!: JsonSchema;

  ngOnChanges(changes: SimpleChanges): void {
    
  }
}

//   renderers = [
//     ...bootstrapRenderers,
//   ];

//   // properties: JsonSchemaProperty[] = [];
//   // fields: AutoField[] = [];
//   // form = new FormGroup({});

//   ngOnChanges(changes: SimpleChanges): void {
//     const schema = changes["schema"];
//     if (schema) {
//       const current = schema.currentValue as object;
//       console.log(current);
//       // this.parseSchema(current);
//       // this.fields = schemaToFields(current);
//       // this.form = fieldsToFormGroup(this.fields);
//     }
//   }

//   // parseSchema(schema: JsonSchema): JsonSchemaProperty {
//   //   switch (schema.type) {
//   //     case "object":
//   //       const keys = Object.keys(schema.properties ?? {});
//   //       break;

//   //     case "array":
//   //       break;

//   //     case "null":
//   //     case "number":
//   //     case "integer":
//   //     case "string":
//   //       break;
//   //   }
//   // }
// }

// @Component({
//   selector: "app-bootstrap-string-control",
//   template: `
//     <div class="mb-3" [hidden]="hidden">
//       <label class="form-label" [attr.for]="id">
//         {{ label }}
//       </label>

//       <input
//         class="form-control"
//         [id]="id"
//         type="text"
//         [value]="data ?? ''"
//         [disabled]="!enabled"
//         (input)="onChange($any($event.target).value)" />

//       @if (description) {
//         <div class="form-text">
//           {{ description }}
//         </div>
//       }

//       @if (error) {
//         <div class="invalid-feedback d-block">
//           {{ error }}
//         </div>
//       }
//     </div>
//   `,
// })
// export class BootstrapStringControlRenderer extends JsonFormsControl {
//   constructor(jsonFormsService: JsonFormsAngularService) {
//     super(jsonFormsService);
//   }
// }

// const renderers: JsonFormsRendererRegistryEntry[] = [
//   {
//     tester: rankWith(2, isStringControl),
//     renderer: BootstrapStringControlRenderer,
//   },
// ];

// export function schemaToFields(
//   schema: JsonSchema,
//   path: string[] = []
// ): AutoField[] {
//   if (!schema.properties) return [];

//   const required = new Set(schema.required ?? []);

//   return Object.entries(schema.properties).flatMap(([key, childSchema]) => {
//     const field = schemaToField(childSchema, {
//       key,
//       path: [...path, key],
//       required: required.has(key),
//     });

//     return field ? [field] : [];
//   });
// }

// interface FieldContext {
//   key: string;
//   path: string[];
//   required: boolean;
// }

// export function schemaToField(
//   schema: JsonSchema,
//   ctx: FieldContext
// ): AutoField | null {
//   const type = getPrimaryType(schema);

//   const base = {
//     key: ctx.key,
//     path: ctx.path,
//     label: schema.title ?? humanize(ctx.key),
//     description: schema.description,
//     required: ctx.required,
//     defaultValue: schema.default,
//     readonly: Boolean(schema["readOnly"] ?? schema["readonly"]),
//     schema,
//   };

//   if (schema.enum) {
//     return {
//       ...base,
//       kind: "select",
//       options: schema.enum.map((value) => ({
//         label: String(value),
//         value,
//       })),
//     };
//   }

//   switch (type) {
//     case "string":
//       return {
//         ...base,
//         kind: "text",
//         minLength: schema.minLength,
//         maxLength: schema.maxLength,
//         pattern: schema.pattern,
//         format: schema.format,
//       };

//     case "number":
//     case "integer":
//       return {
//         ...base,
//         kind: "number",
//         integer: type === "integer",
//         minimum: schema.minimum,
//         maximum: schema.maximum,
//         multipleOf: schema.multipleOf,
//       };

//     case "boolean":
//       return {
//         ...base,
//         kind: "boolean",
//       };

//     case "object":
//       return {
//         ...base,
//         kind: "object",
//         fields: schemaToFields(schema, ctx.path),
//       };

//     case "array":
//       return {
//         ...base,
//         kind: "array",
//         item: schema.items
//           ? schemaToField(schema.items as JsonSchema, {
//             key: ctx.key + "Item",
//             path: [...ctx.path, "*"],
//             required: false,
//           }) ?? undefined
//           : undefined,
//         minItems: schema.minItems,
//         maxItems: schema.maxItems,
//       };

//     default:
//       return null;
//   }
// }

// function getPrimaryType(schema: JsonSchema): string | undefined {
//   if (typeof schema.type === "string") return schema.type;

//   if (Array.isArray(schema.type)) {
//     // Ignore nullability for rendering purposes.
//     return schema.type.find((t) => t !== "null");
//   }

//   if (schema.properties) return "object";
//   if (schema.items) return "array";
//   if (schema.enum) return "string";

//   return undefined;
// }

// function humanize(key: string): string {
//   return key
//     .replace(/([A-Z])/g, " $1")
//     .replace(/[_-]+/g, " ")
//     .replace(/^./, (c) => c.toUpperCase());
// }

// export type AutoField =
//   | TextField
//   | NumberField
//   | BooleanField
//   | SelectField
//   | ObjectField
//   | ArrayField;

// interface BaseField {
//   key: string;
//   path: string[];
//   label: string;
//   description?: string;
//   required: boolean;
//   defaultValue?: unknown;
//   readonly?: boolean;

//   /**
//    * Keep the original schema around for validation, debugging,
//    * custom renderers, or future extension.
//    */
//   schema: JsonSchema;
// }

// export interface TextField extends BaseField {
//   kind: "text";
//   minLength?: number;
//   maxLength?: number;
//   pattern?: string;
//   format?: string;
// }

// export interface NumberField extends BaseField {
//   kind: "number";
//   integer?: boolean;
//   minimum?: number;
//   maximum?: number;
//   multipleOf?: number;
// }

// export interface BooleanField extends BaseField {
//   kind: "boolean";
// }

// export interface SelectField extends BaseField {
//   kind: "select";
//   options: Array<{
//     label: string;
//     value: unknown;
//   }>;
// }

// export interface ObjectField extends BaseField {
//   kind: "object";
//   fields: AutoField[];
// }

// export interface ArrayField extends BaseField {
//   kind: "array";
//   item?: AutoField;
//   minItems?: number;
//   maxItems?: number;
// }

// export function fieldsToFormGroup(fields: AutoField[]): FormGroup {
//   const controls: Record<string, any> = {};

//   for (const field of fields) {
//     controls[field.key] = fieldToControl(field);
//   }

//   return new FormGroup(controls);
// }

// function fieldToControl(field: AutoField) {
//   switch (field.kind) {
//     case "object":
//       return fieldsToFormGroup(field.fields);

//     case "array":
//       return new FormArray([]);

//     default:
//       return new FormControl(field.defaultValue ?? null, validatorsFor(field));
//   }
// }

// function validatorsFor(field: AutoField): ValidatorFn[] {
//   const validators: ValidatorFn[] = [];

//   if (field.required) {
//     validators.push(Validators.required);
//   }

//   if (field.kind === "text") {
//     if (field.minLength !== undefined) {
//       validators.push(Validators.minLength(field.minLength));
//     }

//     if (field.maxLength !== undefined) {
//       validators.push(Validators.maxLength(field.maxLength));
//     }

//     if (field.pattern) {
//       validators.push(Validators.pattern(field.pattern));
//     }
//   }

//   if (field.kind === "number") {
//     if (field.minimum !== undefined) {
//       validators.push(Validators.min(field.minimum));
//     }

//     if (field.maximum !== undefined) {
//       validators.push(Validators.max(field.maximum));
//     }
//   }

//   return validators;
// }

// type UiField =
//   | {
//     kind: "text";
//     key: string;
//     label: string;
//     required?: boolean;
//     minLength?: number;
//     maxLength?: number;
//     pattern?: string;
//     defaultValue?: string;
//   }
//   | {
//     kind: "number";
//     key: string;
//     label: string;
//     required?: boolean;
//     integer?: boolean;
//     min?: number;
//     max?: number;
//     defaultValue?: number;
//   }
//   | {
//     kind: "checkbox";
//     key: string;
//     label: string;
//     defaultValue?: boolean;
//   }
//   | {
//     kind: "select";
//     key: string;
//     label: string;
//     required?: boolean;
//     options: unknown[];
//     defaultValue?: unknown;
//   }
//   | {
//     kind: "object";
//     key: string;
//     label: string;
//     fields: UiField[];
//   }
//   | {
//     kind: "array";
//     key: string;
//     label: string;
//     item: UiField;
//   };

// function normalizeSchema(
//   schema: JsonSchema,
//   key = "",
//   required = false
// ): UiField {
//   const label = schema.title ?? key;

//   if (schema.enum) {
//     return {
//       kind: "select",
//       key,
//       label,
//       required,
//       options: schema.enum,
//       defaultValue: schema.default,
//     };
//   }

//   const type = Array.isArray(schema.type)
//     ? schema.type.find((t: string) => t !== "null")
//     : schema.type;

//   switch (type) {
//     case "string":
//       return {
//         kind: "text",
//         key,
//         label,
//         required,
//         minLength: schema.minLength,
//         maxLength: schema.maxLength,
//         pattern: schema.pattern,
//         defaultValue: schema.default as string,
//       };

//     case "number":
//     case "integer":
//       return {
//         kind: "number",
//         key,
//         label,
//         required,
//         integer: type === "integer",
//         min: schema.minimum,
//         max: schema.maximum,
//         defaultValue: schema.default as number,
//       };

//     case "boolean":
//       return {
//         kind: "checkbox",
//         key,
//         label,
//         defaultValue: schema.default as boolean,
//       };

//     case "object": {
//       const requiredKeys = new Set(schema.required ?? []);

//       return {
//         kind: "object",
//         key,
//         label,
//         fields: Object.entries(schema.properties ?? {}).map(
//           ([childKey, childSchema]) =>
//             normalizeSchema(
//               childSchema,
//               childKey,
//               requiredKeys.has(childKey)
//             )
//         ),
//       };
//     }

//     case "array":
//       return {
//         kind: "array",
//         key,
//         label,
//         item: normalizeSchema(schema.items as JsonSchema, "item"),
//       };

//     default:
//       return {
//         kind: "text",
//         key,
//         label,
//         required,
//         defaultValue: schema.default as string,
//       };
//   }
// }

// function controlForField(field: UiField): AbstractControl {
//   switch (field.kind) {
//     case "text": {
//       const validators = [];

//       if (field.required) validators.push(Validators.required);
//       if (field.minLength != null) validators.push(Validators.minLength(field.minLength));
//       if (field.maxLength != null) validators.push(Validators.maxLength(field.maxLength));
//       if (field.pattern) validators.push(Validators.pattern(field.pattern));

//       return new FormControl(field.defaultValue ?? "", validators);
//     }

//     case "number": {
//       const validators = [];

//       if (field.required) validators.push(Validators.required);
//       if (field.min != null) validators.push(Validators.min(field.min));
//       if (field.max != null) validators.push(Validators.max(field.max));

//       return new FormControl(field.defaultValue ?? null, validators);
//     }

//     case "checkbox":
//       return new FormControl(field.defaultValue ?? false);

//     case "select": {
//       const validators = field.required ? [Validators.required] : [];
//       return new FormControl(field.defaultValue ?? null, validators);
//     }

//     case "object": {
//       const controls: Record<string, AbstractControl> = {};

//       for (const child of field.fields) {
//         controls[child.key] = controlForField(child);
//       }

//       return new FormGroup(controls);
//     }

//     case "array":
//       return new FormArray([]);
//   }
// }

// function createArrayItemControl(arrayField: Extract<UiField, { kind: "array" }>) {
//   return controlForField(arrayField.item);
// }

// function addArrayItem(array: FormArray, arrayField: Extract<UiField, { kind: "array" }>) {
//   array.push(createArrayItemControl(arrayField));
// }

// interface JsonSchemaBase {
//   name: string;
// }

// interface JsonSchemaNull extends JsonSchemaBase {
//   type: "null";
// }

// interface JsonSchemaStruct extends JsonSchemaBase {
//   type: "boolean" | "number" | "integer" | "string";
//   nullable: boolean;
// }

// interface JsonSchemaObject extends JsonSchemaBase {
//   type: "object";
//   nullable: boolean;
//   properties: JsonSchemaProperty[];
// }

// interface JsonSchemaArray extends JsonSchemaBase {
//   type: "array";
//   nullable: boolean;
//   items: JsonSchemaProperty;
// }

// type JsonSchemaProperty = JsonSchemaNull | JsonSchemaStruct | JsonSchemaObject | JsonSchemaArray;
