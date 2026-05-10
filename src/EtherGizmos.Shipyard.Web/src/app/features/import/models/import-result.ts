import z from "zod";

export const ImportResultZ = z.object({
  kind: z.string(),
  schemaVersion: z.number().int(),
  id: z.any(),
  identifier: z.any(),
  status: z.string(),
  errorMessage: z.string().nullish(),
});

export interface ImportResult extends z.infer<typeof ImportResultZ> { }
