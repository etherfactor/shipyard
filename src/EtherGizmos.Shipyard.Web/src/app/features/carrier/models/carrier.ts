import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { carrierRunbookStepForm, CarrierRunbookStepZ } from "./carrier-runbook-step";
import { carrierStatusRuleForm, CarrierStatusRuleZ } from "./carrier-status-rule";

export const CarrierZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  name: z.string(),
  slug: z.string(),
  steps: z.array(CarrierRunbookStepZ),
  rules: z.array(CarrierStatusRuleZ),
});

export interface Carrier extends z.infer<typeof CarrierZ> { }

export const carrierForm = formFactoryForModel<Carrier>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  name: [model.name, AppValidators.required],
  slug: [model.slug, AppValidators.required],
  steps: $form.array(model.steps?.map(item => carrierRunbookStepForm($form, item)) ?? []),
  rules: $form.array(model.rules?.map(item => carrierStatusRuleForm($form, item)) ?? []),
}));
