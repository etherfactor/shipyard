import { Component, computed, inject, Input, OnChanges, OnInit, signal, SimpleChanges } from "@angular/core";
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, TouchedChangeEvent } from "@angular/forms";
import { JsonFormsModule } from "@jsonforms/angular";
import { NgSelectModule } from "@ng-select/ng-select";
import { filter } from "rxjs";
import { ReadonlyFormDirective } from "../../directives/readonly-form/readonly-form.directive";
import { isArrayType, isBooleanType, isIntegerType, isNullType, isNumberType, isObjectType, isStringType, JsonSchema, JsonSchemaObject, JsonSchemaType } from "../../types/json-schema/json-schema";
import { AppValidators } from "../../utilities/form/form.util";

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
export class JsonSchemaAutoFormComponent implements OnInit, OnChanges {
  private readonly $form = inject(FormBuilder);

  @Input({ required: true }) schema!: JsonSchema;
  @Input({ required: true }) form!: FormGroup;
  @Input() readonly = false;

  protected readonly selfRecordForm = new FormArray<any>([]);

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

  ngOnInit(): void {
    this.selfRecordForm.valueChanges.subscribe((properties: { name: string, value: string }[]) => {
      const form = this.form;

      if (this.dontEmit$$()) return;

      for (const key of Object.keys(form.controls)) {
        const property = properties.find(property => property.name === key);
        if (!property) {
          form.removeControl(key);
        } else {
          if (!(form.controls[property.name] instanceof FormControl)) {
            form.removeControl(property.name);
          }
        }
      }

      const used: Record<string, boolean> = {};
      for (let i = 0; i < properties.length; i++) {
        const property = properties[i];
        if (!used[property.name]) {
          if (!form.controls[property.name]) {
            form.addControl(property.name, new FormControl(undefined, { nonNullable: true, validators: [AppValidators.required] }));
          }

          form.controls[property.name].setValue(property.value ?? "");
          form.controls[property.name].markAsDirty();
        } else {
          let useControl = this.selfRecordForm.controls[i];
          if (useControl instanceof FormGroup) {
            useControl = useControl.controls["name"];
          }
          useControl.setErrors({ name: "Name is a duplicate" });
        }

        used[property.name] = true;
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    let regenForm = false;

    const schema = changes["schema"];
    if (schema) {
      regenForm = true;
      const newSchema = schema.currentValue as JsonSchema;
      if (!isObjectType(newSchema)) {
        throw new Error("Auto-generated JSON schema forms require an object schema");
      }

      this.schema$$.set(newSchema);
    }

    const form = changes["form"];
    if (form) {
      regenForm = true;
      const self = this.schema$$();
      if (self && isObjectType(self) && self.additionalProperties) {
        const newForm = form.currentValue as FormGroup;
        this.setSelfRows(newForm.value);
        newForm.valueChanges.subscribe((value: object) => {
          this.setSelfRows(value);
        });
        newForm.events.pipe(
          filter(event => event instanceof TouchedChangeEvent),
        ).subscribe(() => {
          this.selfRecordForm.markAllAsTouched();
        });
      }
    }

    if (regenForm) {
      const form = this.form;
      const schema = this.schema$$();
      const properties = this.properties$$();

      if (!schema?.additionalProperties) {
        for (const key of Object.keys(form.controls)) {
          const property = properties.find(property => property.name === key);
          if (!property) {
            form.removeControl(key);
          } else {
            if (property.schema.type === JsonSchemaType.Object) {
              if (!(form.controls[property.name] instanceof FormGroup)) {
                form.removeControl(property.name);
              }
            } else if (property.schema.type === JsonSchemaType.Array) {
              if (!(form.controls[property.name] instanceof FormArray)) {
                form.removeControl(property.name);
              }
            } else {
              if (!(form.controls[property.name] instanceof FormControl)) {
                form.removeControl(property.name);
              }
            }
          }
        }
      }

      for (const property of properties) {
        if (property.schema.type === JsonSchemaType.Object) {
          if (!form.controls[property.name]) {
            form.addControl(property.name, new FormGroup({}));
          }
        } else if (property.schema.type === JsonSchemaType.Array) {
          if (!form.controls[property.name]) {
            form.addControl(property.name, new FormArray([]));
          }
        } else {
          if (!form.controls[property.name]) {
            let required = true;
            if (typeof property.schema.type === "object" && property.schema.type.some(type => type === JsonSchemaType.Null)) {
              required = false;
            }
            form.addControl(property.name, new FormControl("", { nonNullable: true, validators: required ? [AppValidators.required] : [] }));
          }
        }
      }
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
  }

  isFormGroup(control: AbstractControl): control is FormGroup {
    return control instanceof FormGroup;
  }

  setSelfRows(value: object) {
    const entries = Object.entries(value).map(entry => ({ name: entry[0], value: entry[1] }));

    const curVal = JSON.stringify(this.selfRecordForm.value);
    const newVal = JSON.stringify(entries);

    if (newVal !== curVal) {
      this.dontEmit(() => {
        this.selfRecordForm.clear();
        for (const entry of entries) {
          this.selfRecordForm.push(this.$form.nonNullable.group({
            name: [entry.name, AppValidators.required],
            value: [entry.value, AppValidators.required],
          }));
        }
      });
    }
  }

  addSelfRow() {
    this.selfRecordForm.push(this.$form.nonNullable.group({
      name: ["", AppValidators.required],
      value: [null, AppValidators.required],
    }));
  }

  removeSelfRow(index: number) {
    this.selfRecordForm.removeAt(index);
  }

  readonly dontEmit$$ = computed(() => this.dontEmitStack$$() > 0);
  private readonly dontEmitStack$$ = signal(0);
  async dontEmit(action: () => void | Promise<void>) {
    this.dontEmitStack$$.set(this.dontEmitStack$$() + 1);
    try {
      const result = action();
      if (result instanceof Promise)
        await result;
    } finally {
      this.dontEmitStack$$.set(this.dontEmitStack$$() - 1);
    }
  }

  isNullType = isNullType;
  isBooleanType = isBooleanType;
  isIntegerType = isIntegerType;
  isNumberType = isNumberType;
  isStringType = isStringType;
  isObjectType = isObjectType;
  isArrayType = isArrayType;
}
