import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { load } from 'js-yaml';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { ToastService } from '../../../../shared/services/toast/toast.service';
import { AppValidators } from '../../../../shared/utilities/form/form.util';
import { ImportResult } from '../../models/import-result';
import { ImportSpec, ImportSpecZ } from '../../models/import-spec';
import { ImportService } from '../../services/import/import.service';

@Component({
  selector: 'app-import-home',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    MonacoEditorModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
    RouterModule,
  ],
  templateUrl: './import-home.component.html',
  styleUrl: './import-home.component.scss',
})
export class ImportHomeComponent {
  private readonly $form = inject(FormBuilder);
  private readonly $import = inject(ImportService);
  private readonly $router = inject(Router);
  private readonly $toast = inject(ToastService);

  form;

  documentTextFile?: string;
  documentTextRaw?: string;
  documentRaw$$ = signal<string>("");
  documentRawJson$$ = signal(false);
  document$$ = signal<ImportSpec | undefined>(undefined);
  result$$ = signal<ImportResult | undefined>(undefined);

  private readonly isSavingStack$$ = signal(0);
  readonly isSaving$$ = computed(() => this.isSavingStack$$() > 0);

  constructor() {
    this.form = this.$form.group({
      sourceType: this.$form.nonNullable.control<"file" | "raw">("file", [AppValidators.required]),
      documentFile: this.$form.nonNullable.control<string>(undefined!, [AppValidators.required]),
      documentRaw: this.$form.nonNullable.control<string>(undefined!, [AppValidators.required]),
    });
  }

  onFileSelect($event: Event) {
    const element = $event.target as HTMLInputElement;
    const fileList = element.files;

    const file = fileList?.[0];
    if (file) {
      const reader = new FileReader();

      reader.onload = () => {
        this.documentTextFile = reader.result as string;
      };

      reader.readAsText(file);
    }
  }

  async tryValidate() {
    let content: string | undefined = undefined;
    if (this.form.value.sourceType === "file") {
      if (this.form.controls.documentFile.invalid) {
        this.form.markAllAsTouched();
        return;
      }
      content = this.documentTextFile!;
    } else {
      if (this.form.controls.documentRaw.invalid) {
        this.form.markAllAsTouched();
        return;
      }
      content = this.form.value.documentRaw!;
    }

    let data: unknown = undefined;
    let isJson = false;
    try {
      data = JSON.parse(content);
      isJson = true;
    } catch { }

    try {
      data = load(content);
    } catch { }

    const parsed = ImportSpecZ.safeParse(data);
    if (!parsed.success) {
      this.$toast.show({
        header: "Invalid Document",
        body: "The selected document is not a valid Shipyard export document.",
        theme: "danger",
      });
      return;
    }

    this.isSavingStack$$.set(this.isSavingStack$$() + 1);
    try {
      const result = await this.$import.verify(content, isJson ? "application/json" : "application/yaml");
      if (result.status === "Error") {
        this.$toast.show({
          header: "Invalid Document",
          body: "The selected document failed validation:\r\n" + result.errorMessage,
          theme: "danger",
        });
        return;
      }
    } finally {
      this.isSavingStack$$.set(this.isSavingStack$$() - 1);
    }

    this.documentRaw$$.set(content);
    this.documentRawJson$$.set(isJson);
    this.document$$.set(parsed.data);
  }

  clear() {
    this.document$$.set(undefined);
    this.result$$.set(undefined);

    this.form.setValue({
      sourceType: "file",
      documentFile: null!,
      documentRaw: null!,
    });
    this.form.markAsUntouched();
  }

  async import() {
    this.isSavingStack$$.set(this.isSavingStack$$() + 1);
    try {
      const document = this.documentRaw$$();
      const result = await this.$import.import(document, this.documentRawJson$$() ? "application/json" : "application/yaml");
      this.result$$.set(result);
    } finally {
      this.isSavingStack$$.set(this.isSavingStack$$() - 1);
    }
  }

  gotoRecord() {
    const result = this.result$$();
    if (!result)
      return;

    this.$router.navigate(["/carriers", result.id]);
  }
}
