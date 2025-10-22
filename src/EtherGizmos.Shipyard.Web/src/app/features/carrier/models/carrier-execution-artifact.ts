import { z } from "zod";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";

export const CarrierExecutionArtifactZ = z.object({
  artifactUri: z.string(),
  contentType: z.string(),
  bytes: z.number().int(),
  stepIndex: z.number().int().nullish(),
});

export interface CarrierExecutionArtifact extends z.infer<typeof CarrierExecutionArtifactZ> { }

export type CarrierExecutionArtifactF = Omit<CarrierExecutionArtifact, "">;

export const carrierExecutionArtifactForm = formFactoryForModel<CarrierExecutionArtifactF>(($form, model) => ({
  artifactUri: [model.artifactUri],
  contentType: [model.contentType],
  bytes: [model.bytes],
  stepIndex: [model.stepIndex],
}));
