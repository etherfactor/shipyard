import { FormControl, FormGroup } from "@angular/forms";
import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { NotificationChannelZ } from "./notification-channel";
import { NotificationEventZ } from "./notification-event";
import { NotificationScheduleZ } from "./notification-schedule";

export const NotificationSubscriptionZ = z.object({
  id: z.number().int(),
  userId: GuidZ,
  notificationEventId: z.string(),
  notificationEvent: z.lazy(() => NotificationEventZ).nullish(),
  notificationChannelId: z.string(),
  notificationChannel: z.lazy(() => NotificationChannelZ).nullish(),
  notificationChannelConfig: z.object({}).loose(),
  notificationScheduleId: z.string(),
  notificationSchedule: z.lazy(() => NotificationScheduleZ).nullish(),
  notificationScheduleConfig: z.object({}).loose(),
  isActive: z.boolean(),
  lastNotificationAt: DateTimeZ.nullish(),
  nextNotificationAt: DateTimeZ.nullish(),
});

export interface NotificationSubscription extends z.infer<typeof NotificationSubscriptionZ> { }

export type NotificationSubscriptionF = Omit<NotificationSubscription, "notificationEvent" | "notificationChannel" | "notificationSchedule">;

export const notificationSubscriptionForm = formFactoryForModel<NotificationSubscriptionF>(($form, model) => ({
  id: [model.id],
  userId: [model.userId],
  notificationEventId: [model.notificationEventId, AppValidators.required],
  notificationChannelId: [model.notificationChannelId, AppValidators.required],
  notificationChannelConfig: convertToFormGroup(model.notificationChannelConfig ?? {}),
  notificationScheduleId: [model.notificationScheduleId, AppValidators.required],
  notificationScheduleConfig: convertToFormGroup(model.notificationScheduleConfig ?? {}),
  isActive: [model.isActive, AppValidators.required],
  lastNotificationAt: [model.lastNotificationAt],
  nextNotificationAt: [model.nextNotificationAt],
}));

function convertToFormGroup(data: any): FormGroup {
  const group: any = {};
  Object.keys(data).forEach(key => {
    if (typeof data[key] === "object" && data[key] !== null && !Array.isArray(data[key])) {
      group[key] = convertToFormGroup(data[key]);
    } else {
      group[key] = new FormControl(data[key]);
    }
  });
  return new FormGroup(group);
}
