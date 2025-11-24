import z from "zod";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";

export const RoleZ = z.object({
  id: z.number().int(),
  name: z.string(),
  description: z.string().nullish(),
});

export interface Role extends z.infer<typeof RoleZ> { }

export type RoleF = Omit<Role, "">;

export const roleForm = formFactoryForModel<RoleF>(($form, model) => ({
  id: [model.id],
  name: [model.name, AppValidators.required],
  description: [model.description],
}));
