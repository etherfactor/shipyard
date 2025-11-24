import z from "zod";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { User, UserZ } from "../../user/models/user";

const RoleZ_base = z.object({
  id: z.number().int(),
  name: z.string(),
  description: z.string().nullish(),
});

type Role_base = z.infer<typeof RoleZ_base> & {
  users?: User[] | null;
}

export const RoleZ: z.ZodType<Role> = RoleZ_base.extend({
  users: z.array(z.lazy(() => UserZ)).nullish(),
});

export interface Role extends Role_base { }

export type RoleF = Omit<Role, "users">;

export const roleForm = formFactoryForModel<RoleF>(($form, model) => ({
  id: [model.id],
  name: [model.name, AppValidators.required],
  description: [model.description],
}));
