import { Component, computed, inject, OnDestroy, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
import { InitializeModal } from '../../../../shared/components/initialize-modal/initialize-modal.component';
import { CarrierExecutionService } from '../../services/carrier-execution/carrier-execution.service';

@Component({
  selector: 'app-artifact-preview-modal',
  imports: [
    FormsModule,
    MonacoEditorModule,
  ],
  templateUrl: './artifact-preview-modal.component.html',
  styleUrl: './artifact-preview-modal.component.scss'
})
export class ArtifactPreviewModalComponent extends InitializeModal<[number, string], 1, 0> implements OnDestroy {

  private readonly $carrierExecution = inject(CarrierExecutionService);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  type?: string;
  textContent!: string;
  imageContent!: string;

  override get defaultClose(): 1 {
    return 1;
  }

  override get defaultDismiss(): 0 {
    return 0;
  }

  override async initialize(executionId: number, uri: string) {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    try {
      const artifact = await this.$carrierExecution.readBinaryArtifact(executionId, uri);
      const useType = artifact.type.split(";")[0];
      this.type = useType;
      if (this.type.startsWith("text/")) {
        const array = new Uint8Array(artifact.buffer);
        const decoder = new TextDecoder("utf-8");
        this.textContent = decoder.decode(array);
      } else if (this.type.startsWith("image/")) {
        const blob = new Blob([artifact.buffer], { type: this.type });
        const url = URL.createObjectURL(blob);
        this.imageContent = url;
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  ngOnDestroy(): void {
    if (this.imageContent) {
      URL.revokeObjectURL(this.imageContent);
    }
  }
}
