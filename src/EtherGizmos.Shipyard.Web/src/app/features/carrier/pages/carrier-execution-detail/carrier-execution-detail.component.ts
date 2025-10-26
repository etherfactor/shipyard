import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime, Duration } from 'luxon';
import { DetailBoxButton, DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { openModal } from '../../../../shared/utilities/modal/modal.util';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { ArtifactPreviewModalComponent } from '../../components/artifact-preview-modal/artifact-preview-modal.component';
import { CarrierExecution } from '../../models/carrier-execution';
import { CarrierExecutionArtifact } from '../../models/carrier-execution-artifact';
import { ExecutionStatusType, getExecutionStatusTypeMetadata } from '../../models/execution-status-type';
import { Log, LogSection, LogZ } from '../../models/log';
import { CarrierExecutionService } from '../../services/carrier-execution/carrier-execution.service';

@Component({
  selector: 'app-carrier-execution-detail',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: './carrier-execution-detail.component.html',
  styleUrl: './carrier-execution-detail.component.scss'
})
export class CarrierExecutionDetailComponent {

  private readonly $carrierExecution = inject(CarrierExecutionService);
  private readonly $modal = inject(NgbModal);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly carrierExecution$$ = signal<CarrierExecution | undefined>(undefined);

  readonly logs$$ = signal<Log[]>([]);
  readonly sections$$ = signal<LogSection[]>([]);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];
    return actions;
  });

  readonly execButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    buttons.push({
      color: "primary",
      text: "View all",
      callback: () => { },
    });

    return buttons;
  });

  readonly sortedArtifacts$$ = computed(() => {
    const map: Record<number, StepArtifactContainer> = {};
    for (const artifact of this.carrierExecution$$()?.artifacts ?? []) {
      map[artifact.stepIndex ?? 0] ??= {
        step: artifact.stepIndex ?? undefined,
        artifacts: [],
      };

      map[artifact.stepIndex ?? 0].artifacts.push(artifact);
    }

    return Object.values(map).sort((a, b) => (a.step ?? 0) - (b.step ?? 0));
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const executionId = this.$route.snapshot.paramMap.get("executionId")!;
    const id = parseInt(executionId);
    this.id$$.set(id);

    this.load();
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load(single?: EntitySingle<CarrierExecution>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$carrierExecution
      .get(id);

    try {
      const exec = single
        .expand("carrier", e => e
          .select("id", "name")
        )
        .execute();
      const data = await exec.data;

      this.carrierExecution$$.set(data);

      const uri = data.artifacts.find(e => e.contentType === "application/x-ndjson")!.artifactUri;
      const logText = await this.$carrierExecution.readTextArtifact(id, uri);

      const logs = logText.split("\n").map(log => log.trim()).filter(log => log).map(log => LogZ.parse(JSON.parse(log)));
      this.logs$$.set(logs);

      const sections: LogSection[] = [];
      let section: LogSection = {} as LogSection;
      for (const log of logs) {
        if (log.properties["FLAG"] === "STEP_START") {
          section = {
            step: parseInt(log.properties["Step"]),
            name: log.properties["StepName"],
            duration: 0,
          };
        }

        if (log.properties["FLAG"] === "STEP_END") {
          section.duration = parseInt(log.properties["StepDuration"]) / 1000.0;
          sections.push(section);
        }
      }

      this.sections$$.set(sections);
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  getExecutionStatusMetadata(statusType: ExecutionStatusType) {
    return getExecutionStatusTypeMetadata(statusType);
  }

  getDiffTime(dateTime1: DateTime | null | undefined, dateTime2: DateTime | null | undefined) {
    if (!dateTime1 || !dateTime2)
      return "—";

    let duration = Duration.fromMillis(dateTime2.toMillis() - dateTime1.toMillis())
      .shiftTo("hours", "minutes", "seconds");

    if (duration.hours === 0) {
      duration = duration.shiftTo("minutes", "seconds");

      if (duration.minutes === 0) {
        duration = duration.shiftTo("seconds");
      }
    }

    return duration.toHuman({ unitDisplay: "short" }).split(",")[0];
  }

  jumpToStep(step: number) {
    const log = document.querySelector(`[data-sy-stepid="${step}"]`);
    log?.scrollIntoView();
  }

  jumpToStepEvent($event: any) {
    try {
      this.jumpToStep(parseInt($event.target.value));
    } catch { }
  }

  async openArtifactPreview(uri: string) {
    await openModal({ modal: this.$modal, options: { size: "xl" } }, ArtifactPreviewModalComponent, this.id$$()!, uri);
  }

  async downloadArtifact(uri: string) {
    const meta = this.carrierExecution$$()!.artifacts.find(e => e.artifactUri === uri)!;
    const artifact = await this.$carrierExecution.readBinaryArtifact(this.id$$()!, uri);
    const blob = new Blob([artifact.buffer], { type: artifact.type });

    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = meta.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url); // Release the object URL
  }
}

interface StepArtifactContainer {
  step: number | undefined;
  artifacts: CarrierExecutionArtifact[];
}
