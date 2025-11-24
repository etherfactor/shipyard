import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";

export const GroupZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
});

export interface Group extends z.infer<typeof GroupZ> { }

export type GroupF = Omit<Group, "">;

export const groupForm = formFactoryForModel<GroupF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  name: [model.name, AppValidators.required],
  description: [model.description],
}));
