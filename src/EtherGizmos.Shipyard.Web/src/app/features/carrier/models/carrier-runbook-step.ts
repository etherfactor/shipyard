import { z } from "zod";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { StepType } from "./step-type";

export const CarrierRunbookStepZ = z.object({
  stepType: z.nativeEnum(StepType),
});

export interface CarrierRunbookStep extends z.infer<typeof CarrierRunbookStepZ> { }

export const carrierRunbookStepForm = formFactoryForModel<CarrierRunbookStep>(($form, model) => ({
  stepType: [model.stepType],
}));
