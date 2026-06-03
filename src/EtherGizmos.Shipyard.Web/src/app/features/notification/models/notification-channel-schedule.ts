import z from "zod";
import { NotificationChannelZ } from "./notification-channel";
import { NotificationScheduleZ } from "./notification-schedule";

export const NotificationChannelScheduleZ = z.object({
  notificationChannelId: z.string(),
  notificationChannel: z.lazy(() => NotificationChannelZ).nullish(),
  notificationScheduleId: z.string(),
  notificationSchedule: z.lazy(() => NotificationScheduleZ).nullish(),
});

export interface NotificationChannelSchedule extends z.infer<typeof NotificationChannelScheduleZ> { }
