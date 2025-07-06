import { z } from "zod";
import { formFactoryForModel, TypedFormGroup } from "../../../shared/utilities/form/form.util";
import { StepType } from "./step-type";
import { FormBuilder } from "@angular/forms";

const CarrierRunbookStepZ_base = z.object({
  stepType: z.nativeEnum(StepType),
  from: z.string().nullish(),
  selector: z.string().nullish(),
  to: z.string().nullish(),
  trim: z.boolean().nullish(),
  url: z.string().nullish(),
  value: z.string().nullish(),
  var: z.string().nullish(),
});

type CarrierRunbookStep_base = z.infer<typeof CarrierRunbookStepZ_base> & {
  steps?: CarrierRunbookStep[] | null;
}

export const CarrierRunbookStepZ: z.ZodType<CarrierRunbookStep> = CarrierRunbookStepZ_base.extend({
  steps: z.array(z.lazy(() => CarrierRunbookStepZ)).nullish(),
});

export interface CarrierRunbookStep extends CarrierRunbookStep_base { }

let form: ($form: FormBuilder, model: CarrierRunbookStep) => TypedFormGroup<CarrierRunbookStep>;
export const carrierRunbookStepForm = formFactoryForModel<CarrierRunbookStep>(($form, model) => ({
  stepType: [model.stepType],
  from: [model.from],
  selector: [model.selector],
  steps: model.steps ? $form.nonNullable.array(model.steps.map(item => form($form, item))) : undefined,
  to: [model.to],
  trim: [model.trim],
  url: [model.url],
  value: [model.value],
  var: [model.var],
}));

form = carrierRunbookStepForm;
