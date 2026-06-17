import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { NotificationStatusType } from "./notification-status-type";
import { NotificationSubscriptionZ } from "./notification-subscription";

export const NotificationZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  sentAt: DateTimeZ.nullish(),
  statusType: z.enum(NotificationStatusType),
  notificationSubscriptionId: z.number().int(),
  notificationSubscription: z.lazy(() => NotificationSubscriptionZ).nullish(),
  //isRead: z.boolean(),
  payload: z.object({}).loose(),
});

export interface Notification extends z.infer<typeof NotificationZ> { }
