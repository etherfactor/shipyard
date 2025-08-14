import { FormBuilder } from "@angular/forms";
import { z } from "zod";
import { formFactoryForModel, TypedFormGroup } from "../../../shared/utilities/form/form.util";
import { StepType } from "./step-type";

const CarrierRunbookStepZ_base = z.object({
  stepType: z.nativeEnum(StepType),
  from: z.string().nullish(),
  isRegex: z.boolean().nullish(),
  name: z.string().nullish(),
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
  isRegex: [model.isRegex],
  name: [model.name],
  selector: [model.selector],
  steps: $form.nonNullable.array(model.steps?.map(item => form($form, item)) ?? []),
  to: [model.to],
  trim: [model.trim],
  url: [model.url],
  value: [model.value],
  var: [model.var],
}));

form = carrierRunbookStepForm;

export const fieldsByStepType: Record<StepType, { field: keyof CarrierRunbookStep, required: boolean }[]> = {
  Click: [
    {
      field: "selector",
      required: true,
    },
  ],
  ExtractList: [
    {
      field: "selector",
      required: true,
    },
    {
      field: "var",
      required: true,
    },
    {
      field: "steps",
      required: true,
    },
  ],
  Extract: [
    {
      field: "selector",
      required: true,
    },
    {
      field: "var",
      required: true,
    },
    {
      field: "trim",
      required: false,
    },
  ],
  Navigate: [
    {
      field: "url",
      required: true,
    },
  ],
  Replace: [
    {
      field: "var",
      required: true,
    },
    {
      field: "from",
      required: true,
    },
    {
      field: "to",
      required: true,
    },
    {
      field: "isRegex",
      required: false,
    },
    {
      field: "trim",
      required: false,
    },
  ],
  Return: [
    {
      field: "name",
      required: true,
    },
    {
      field: "var",
      required: true,
    },
  ],
  Send: [
    {
      field: "selector",
      required: true,
    },
    {
      field: "value",
      required: true,
    },
  ],
  Set: [
    {
      field: "var",
      required: true,
    },
    {
      field: "value",
      required: true,
    },
    {
      field: "trim",
      required: false,
    },
  ],
  WaitFor: [
    {
      field: "selector",
      required: true,
    },
  ],
};
