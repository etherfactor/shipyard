import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { StatusType } from "../../package/models/status-type";

export const CarrierStatusRuleZ = z.object({
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  pattern: z.string(),
  statusType: z.nativeEnum(StatusType),
  priority: z.number().int(),
  isActive: z.boolean(),
});

export interface CarrierStatusRule extends z.infer<typeof CarrierStatusRuleZ> { }

export const carrierStatusRuleForm = formFactoryForModel<CarrierStatusRule>(($form, model) => ({
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  pattern: [model.pattern, AppValidators.required],
  statusType: [model.statusType, AppValidators.required],
  priority: [model.priority, AppValidators.required],
  isActive: [model.isActive, AppValidators.required],
}));
