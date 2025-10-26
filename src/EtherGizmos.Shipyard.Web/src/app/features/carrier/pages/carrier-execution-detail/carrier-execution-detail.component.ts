import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EntitySingle } from '@ethergizmos/odata-fluent-client';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateTime, Duration } from 'luxon';
import { DetailBoxButton, DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { StatusType, getStatusTypeMetadata } from '../../../package/models/status-type';
import { CarrierExecution } from '../../models/carrier-execution';
import { ExecutionStatusType, getExecutionStatusTypeMetadata } from '../../models/execution-status-type';
import { Log, LogZ } from '../../models/log';
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
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly carrierExecution$$ = signal<CarrierExecution | undefined>(undefined);

  readonly logs$$ = signal<Log[]>([]);

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
      const logText = await this.$carrierExecution.readArtifact(id, uri).execute().data;

      const logs = logText.split("\n").map(log => log.trim()).filter(log => log).map(log => LogZ.parse(JSON.parse(log)));
      this.logs$$.set(logs);
      console.log(logs);
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
}
