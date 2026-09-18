import z from "zod";

export const NotificationChannelZ = z.object({
  id: z.string(),
  name: z.string(),
  configSchema: z.object({}).loose(),
});

export interface NotificationChannel extends z.infer<typeof NotificationChannelZ> { }
