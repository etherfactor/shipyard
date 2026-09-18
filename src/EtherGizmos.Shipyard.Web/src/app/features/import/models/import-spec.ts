import z from "zod";

export const ImportSpecZ = z.object({
  kind: z.string(),
  schemaVersion: z.number().int(),
  metadata: z.object({}).passthrough().nullish(),
  data: z.object({}).passthrough(),
});

export interface ImportSpec extends z.infer<typeof ImportSpecZ> { }
