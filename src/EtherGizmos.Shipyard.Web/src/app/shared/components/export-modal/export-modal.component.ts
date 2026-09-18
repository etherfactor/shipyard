import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { InitializeModal } from '../initialize-modal/initialize-modal.component';

@Component({
  selector: 'app-export-modal',
  imports: [
    FormsModule,
    MonacoEditorModule,
  ],
  templateUrl: './export-modal.component.html',
  styleUrl: './export-modal.component.scss',
})
export class ExportModalComponent extends InitializeModal<[string, Promise<string>, string], void> {
  title!: string;
  content!: string;
  fileName!: string;
  isJson = false;

  private readonly isLoadingStack$$ = signal(0);
  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);

  override get defaultClose(): void {
    return;
  }

  override get defaultDismiss(): void {
    return;
  }

  override initialize(title: string, content: Promise<string>, fileName: string): void {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    this.title = title;
    this.fileName = fileName;

    (async () => {
      this.content = await content;
      try {
        JSON.parse(this.content);
        this.isJson = true;
      } catch { }
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    })();
  }

  download() {
    const blob = new Blob([this.content], {
      type: this.isJson
        ? "application/json"
        : "application/yaml",
    });

    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = this.fileName;
    link.style.display = "none";

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
}
