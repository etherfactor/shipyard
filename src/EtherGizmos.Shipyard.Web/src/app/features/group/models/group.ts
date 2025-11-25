import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { User, UserZ } from "../../user/models/user";

const GroupZ_base = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ.nullish(),
  name: z.string(),
  description: z.string().nullish(),
});

type Group_base = z.infer<typeof GroupZ_base> & {
  users?: User[] | null;
};

export const GroupZ: z.ZodType<Group> = GroupZ_base.extend({
  users: z.array(z.lazy(() => UserZ)).nullish(),
});

export interface Group extends Group_base { }

export type GroupF = Omit<Group, "users">;

export const groupForm = formFactoryForModel<GroupF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  name: [model.name, AppValidators.required],
  description: [model.description],
}));
