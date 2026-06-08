import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";

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
  channelConfig: $form.nonNullable.group({}),
  scheduleType: [model.scheduleType, AppValidators.required],
  scheduleConfig: $form.nonNullable.group({}),
  isActive: [model.isActive, AppValidators.required],
  lastNotificationAt: [model.lastNotificationAt],
  nextNotificationAt: [model.nextNotificationAt],
}));
