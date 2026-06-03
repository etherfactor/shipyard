import z from "zod";

export const NotificationScheduleZ = z.object({
  id: z.string(),
  name: z.string(),
  configSchema: z.object({}).passthrough(),
});

export interface NotificationSchedule extends z.infer<typeof NotificationScheduleZ> { }
