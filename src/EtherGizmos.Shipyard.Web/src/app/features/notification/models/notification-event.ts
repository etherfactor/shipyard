import z from "zod";
import { NotificationChannelScheduleZ } from "./notification-channel-schedule";

export const NotificationEventZ = z.object({
  id: z.string(),
  name: z.string(),
  supports: z.array(z.lazy(() => NotificationChannelScheduleZ)),
});

export interface NotificationEvent extends z.infer<typeof NotificationEventZ> { }
