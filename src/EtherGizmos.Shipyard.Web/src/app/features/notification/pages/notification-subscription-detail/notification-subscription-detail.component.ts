import { Component, computed, effect, inject, OnInit, signal } from "@angular/core";
import { FormBuilder, FormControl, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { EntitySingle } from "@ethergizmos/odata-fluent-client";
import { NgSelectModule } from "@ng-select/ng-select";
import { DetailBoxButton, DetailBoxComponent } from "../../../../shared/components/detail-box/detail-box.component";
import { DetailHeaderComponent } from "../../../../shared/components/detail-header/detail-header.component";
import { InputLuxonDatetimeComponent } from "../../../../shared/components/input-luxon-datetime/input-luxon-datetime.component";
import { JsonSchemaAutoFormComponent } from "../../../../shared/components/json-schema-auto-form/json-schema-auto-form.component";
import { ReadonlyFormDirective } from "../../../../shared/directives/readonly-form/readonly-form.directive";
import { NavbarActionService } from "../../../../shared/services/navbar-action/navbar-action.service";
import { JsonSchema, JsonSchemaZ } from "../../../../shared/types/json-schema/json-schema";
import { Bound } from "../../../../shared/utilities/bound/bound.util";
import { FilterValue } from "../../../../shared/utilities/filter/filter.util";
import { AppValidators, getAllFormValues, getDirtyFormValues, TypedFormGroup } from "../../../../shared/utilities/form/form.util";
import { NavbarAction } from "../../../app/components/navbar-action/navbar-action.component";
import { Notification } from "../../models/notification";
import { NotificationChannel } from "../../models/notification-channel";
import { NotificationEvent } from "../../models/notification-event";
import { NotificationSchedule } from "../../models/notification-schedule";
import { NotificationSubscription, NotificationSubscriptionF, notificationSubscriptionForm } from "../../models/notification-subscription";
import { NotificationChannelTheme, NotificationEventTheme, NotificationScheduleTheme, NotificationTheme } from "../../models/notification-theme";
import { NotificationMetaService } from "../../services/notification-meta/notification-meta.service";
import { NotificationSubscriptionService } from "../../services/notification-subscription/notification-subscription.service";

@Component({
  selector: "app-notification-subscription-detail",
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    JsonSchemaAutoFormComponent,
    InputLuxonDatetimeComponent,
    NgSelectModule,
    ReactiveFormsModule,
    ReadonlyFormDirective,
  ],
  templateUrl: "./notification-subscription-detail.component.html",
  styleUrl: "./notification-subscription-detail.component.scss",
})
export class NotificationSubscriptionDetailComponent implements OnInit {
  private readonly $form = inject(FormBuilder);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $notificationMeta = inject(NotificationMetaService);
  private readonly $notificationSubscription = inject(NotificationSubscriptionService);
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly subscription$$ = signal<NotificationSubscription>({} as NotificationSubscription);
  readonly form$$ = signal<TypedFormGroup<NotificationSubscriptionF> | undefined>(undefined);

  get cronFormControl() {
    return this.form$$()?.controls.notificationScheduleConfig.controls["cronExpression"] as unknown as FormControl<string>
  }

  readonly isEditing$$ = signal(false);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];

    if (!this.isLoading$$()) {
      if (!this.isEditing$$()) {
        actions.push({
          icon: "bi-pencil",
          label: "Edit",
          callback: this.onEdit,
        });
        actions.push({
          icon: "bi-trash",
          label: "Delete",
          callback: this.onDelete,
        });
      } else {
        actions.push({
          icon: "bi-save",
          label: "Save",
          callback: this.onSave,
        });
        actions.push({
          icon: "bi-x-square",
          label: "Cancel",
          callback: this.onCancel,
        });
      }
    }

    return actions;
  });

  readonly channelSchema$$ = signal<JsonSchema | undefined>(undefined);
  readonly isChannelSchemaEmpty$$ = computed(() => {
    const schema = this.channelSchema$$();
    if (!schema || Object.keys(schema).length === 1) {
      return true;
    }

    return false;
  });

  initPromise!: Promise<unknown>;

  readonly events$$ = signal<NotificationEvent[]>([]);
  readonly eventEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.events$$().map(item =>
      [this.formatStyleSpan(NotificationEventTheme, item.id) + " " + this.lookupName(this.events$$(), item.id), item.id]
    );
  });

  readonly channels$$ = signal<NotificationChannel[]>([]);
  readonly channelEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.channels$$().map(item =>
      [this.formatStyleSpan(NotificationChannelTheme, item.id) + " " + this.lookupName(this.channels$$(), item.id), item.id]
    );
  });

  readonly schedules$$ = signal<NotificationSchedule[]>([]);
  readonly scheduleEnum$$ = computed<[string, FilterValue][]>(() => {
    return this.schedules$$().map(item =>
      [this.formatStyleSpan(NotificationScheduleTheme, item.id) + " " + this.lookupName(this.schedules$$(), item.id), item.id]
    );
  });

  readonly delivery$$ = signal<Notification | undefined>(undefined);

  readonly deliveryButtons$$ = computed<DetailBoxButton[]>(() => {
    const buttons: DetailBoxButton[] = [];

    if (!this.isEditing$$()) {
      buttons.push({
        color: "primary",
        text: "View all",
        callback: this.viewDeliveries,
      });
    }

    return buttons;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit() {
    const e = this.loadEvents();
    const c = this.loadChannels();
    const s = this.loadSchedules();
    this.initPromise = Promise.all([e, c, s]);

    const subscriptionId = this.$route.snapshot.paramMap.get("subscriptionId");
    if (subscriptionId) {
      const id = parseInt(subscriptionId);
      this.id$$.set(id);

      this.load();
    } else {
      this.subscription$$.set({ isActive: true } as NotificationSubscription);

      this.onEdit();
      this.init();
    }
  }

  async loadEvents() {
    await this.doWork(async () => {
      const events = await this.$notificationMeta.events.search()
        .orderBy("id")
        .execute()
        .data;

      this.events$$.set(events);
    });
  }

  async loadChannels() {
    await this.doWork(async () => {
      const channels = await this.$notificationMeta.channels.search()
        .orderBy("id")
        .execute()
        .data;

      this.channels$$.set(channels);
    });
  }

  async loadSchedules() {
    await this.doWork(async () => {
      const schedules = await this.$notificationMeta.schedules.search()
        .orderBy("id")
        .execute()
        .data;

      this.schedules$$.set(schedules);
    });
  }

  private async load(single?: EntitySingle<NotificationSubscription>) {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    single ??= this.$notificationSubscription
      .get(id);

    try {
      const exec = single
        .execute();
      const data = await exec.data;
      await this.initPromise;

      this.subscription$$.set(data);
      this.init();

      // try {
      //   this.isLoadingExecStack$$.set(this.isLoadingExecStack$$() + 1);

      //   const exec = await this.$carrierExecution.search()
      //     .filter(e =>
      //       o.and(
      //         o.eq(
      //           e.prop("carrierId"),
      //           o.int(id)
      //         ),
      //         o.ne(
      //           e.prop("startedAt"),
      //           o.null()
      //         )
      //       )
      //     )
      //     .orderBy("startedAt", "desc")
      //     .execute()
      //     .data;

      //   this.exec$$.set(exec[0]);
      // } finally {
      //   this.isLoadingExecStack$$.set(this.isLoadingExecStack$$() - 1);
      // }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  private init() {
    const form = notificationSubscriptionForm(this.$form, this.subscription$$());
    form.controls.notificationChannelId.valueChanges.subscribe(channelId => {
      const channelSchema = this.channels$$().find(channel => channel.id === channelId)?.configSchema;
      if (channelSchema) {
        this.channelSchema$$.set(JsonSchemaZ.parse(channelSchema));
      } else {
        this.channelSchema$$.set(undefined);
      }
    });

    form.controls.notificationScheduleId.valueChanges.subscribe(value => {
      if (value === "digest") {
        form.controls.notificationScheduleConfig.controls["cronExpression"] ??= new FormControl("", { nonNullable: true, validators: [AppValidators.required] }) as any;
      } else {
        delete form.controls.notificationScheduleConfig.controls["cronExpression"];
      }
    });

    this.form$$.set(form);
    form.controls.isActive.markAsDirty();
    if (this.isEditing$$()) {
      form.enable();
    } else {
      form.disable();
    }
  }

  @Bound viewDeliveries() {
    this.$router.navigate(["/notifications", "subscriptions", 0, "deliveries"]);
  }

  async doWork(action: () => void | Promise<void>) {
    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    try {
      const result = action();
      if (result instanceof Promise) {
        await result;
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  formatStyleSpan(themes: Record<string, NotificationTheme>, id: string) {
    const theme = themes[id];
    if (theme) {
      return `<span class="bi ${theme.iconClass} ${theme.colorClass}"></span>`;
    } else {
      return `<span class="bi bi-question-circle text-secondary"></span>`;
    }
  }

  lookupName(names: { id: string, name: string }[], id: string) {
    return names.find(e => e.id === id)?.name ?? id;
  }

  @Bound async onEdit() {
    this.isEditing$$.set(true);
    this.form$$()?.enable();
  }

  @Bound onDelete() {

  }

  @Bound async onSave() {
    const record = this.subscription$$();
    const form = this.form$$();

    if (!record || !form)
      return;

    if (form.invalid) {
      form.markAsUntouched();
      form.markAllAsTouched();
      return;
    }

    if (form.value.notificationScheduleId === "digest" && !this.cronFormControl?.value) {
      this.cronFormControl?.markAsTouched();
      return;
    }

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);
    try {
      const data = getDirtyFormValues(form);

      const value = getAllFormValues(form);

      const curChannelVal = (JSON.stringify(this.subscription$$().notificationChannelConfig) ?? "{}").replace(/""/, "null");
      const newChannelVal = (JSON.stringify(value.notificationChannelConfig) ?? "{}").replace(/""/, "null");
      if (newChannelVal !== curChannelVal) {
        data.notificationChannelConfig = value.notificationChannelConfig;
      }

      const curScheduleVal = (JSON.stringify(this.subscription$$().notificationScheduleConfig) ?? "{}").replace(/""/, "null");
      const newScheduleVal = (JSON.stringify(value.notificationScheduleConfig) ?? "{}").replace(/""/, "null");
      if (newScheduleVal !== curScheduleVal) {
        data.notificationScheduleConfig = value.notificationScheduleConfig;
      }

      if (this.id$$()) {
        const single = this.$notificationSubscription.update(record.id, data);
        await this.load(single);
        this.onCancel();
      } else {
        const create = this.$notificationSubscription.create(data).execute();
        const created = await create.data;
        this.$router.navigate(["/notifications", "subscriptions", created.id]);
      }
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  @Bound onCancel() {
    if (this.id$$()) {
      this.isEditing$$.set(false);
      this.init();
    } else {
      this.$router.navigate(["/notifications", "subscriptions"]);
    }
  }

  NotificationEventTheme = NotificationEventTheme;
  NotificationChannelTheme = NotificationChannelTheme;
  NotificationScheduleTheme = NotificationScheduleTheme;
  JSON = JSON;
}
