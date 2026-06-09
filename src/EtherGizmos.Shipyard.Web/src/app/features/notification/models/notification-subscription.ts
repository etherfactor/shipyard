import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { FormControl, FormGroup } from "@angular/forms";

export const NotificationSubscriptionZ = z.object({
  id: z.number().int(),
  userId: GuidZ,
  eventType: z.string(),
  channelKey: z.string(),
  channelConfig: z.object({}).loose(),
  scheduleType: z.string(),
  scheduleConfig: z.object({}).loose(),
  isActive: z.boolean(),
  lastNotificationAt: DateTimeZ.nullish(),
  nextNotificationAt: DateTimeZ.nullish(),
});

export interface NotificationSubscription extends z.infer<typeof NotificationSubscriptionZ> { }

export type NotificationSubscriptionF = Omit<NotificationSubscription, "">;

export const notificationSubscriptionForm = formFactoryForModel<NotificationSubscription>(($form, model) => ({
  id: [model.id],
  userId: [model.userId],
  eventType: [model.eventType, AppValidators.required],
  channelKey: [model.channelKey, AppValidators.required],
  channelConfig: convertToFormGroup(model.channelConfig ?? {}),
  scheduleType: [model.scheduleType, AppValidators.required],
  scheduleConfig: convertToFormGroup(model.scheduleConfig ?? {}),
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
