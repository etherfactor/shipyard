import { Component, computed, Input, OnChanges, signal, SimpleChanges } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { JsonFormsModule } from "@jsonforms/angular";
import { NgSelectModule } from "@ng-select/ng-select";
import { ReadonlyFormDirective } from "../../directives/readonly-form/readonly-form.directive";
import { isArrayType, isBooleanType, isIntegerType, isNullType, isNumberType, isObjectType, isStringType, JsonSchema, JsonSchemaObject, JsonSchemaType } from "../../types/json-schema/json-schema";

@Component({
  selector: "json-schema-auto-form",
  imports: [
    JsonFormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: "./json-schema-auto-form.component.html",
  styleUrl: "./json-schema-auto-form.component.scss",
})
export class JsonSchemaAutoFormComponent implements OnChanges {
  @Input({ required: true }) schema!: JsonSchema;
  @Input() readonly = false;

  readonly schema$$ = signal<JsonSchemaObject | undefined>(undefined);
  readonly properties$$ = computed(() => {
    const schema = this.schema$$();
    if (!schema) return [];
    if (!schema.properties) return [];

    const keys = Object.keys(schema.properties);
    return keys.map(key => ({
      name: key,
      schema: schema.properties![key],
    }));
  });

  ngOnChanges(changes: SimpleChanges): void {
    const schema = changes["schema"];
    if (schema) {
      const newSchema = schema.currentValue as JsonSchema;
      if (!isObjectType(newSchema)) {
        throw new Error("Auto-generated JSON schema forms require an object schema");
      }

      this.schema$$.set(newSchema);
    }
  }

  isRequired(schema: JsonSchema) {
    if (typeof schema.type === "object") {
      return schema.type.every(type => type !== JsonSchemaType.Null);
    }
    return schema.type !== JsonSchemaType.Null;
  }

  readable(str: string): string {
    return str
      .replace(/([A-Z])/g, " $1")
      .replace(/^./, (firstChar) => firstChar.toUpperCase())
      .trim();
  };

  isNullType = isNullType;
  isBooleanType = isBooleanType;
  isIntegerType = isIntegerType;
  isNumberType = isNumberType;
  isStringType = isStringType;
  isObjectType = isObjectType;
  isArrayType = isArrayType;
}
