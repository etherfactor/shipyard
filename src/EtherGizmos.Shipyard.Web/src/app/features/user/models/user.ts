import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { GroupZ } from "../../group/models/group";
import { RoleZ } from "../../role/models/role";

export const UserZ = z.object({
  id: GuidZ,
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ.nullish(),
  username: z.string(),
  password: z.string(),
  emailAddress: z.string().nullish(),
  givenName: z.string().nullish(),
  familyName: z.string().nullish(),
  fullName: z.string().nullish(),
  groupId: z.number().int(),
  group: z.lazy(() => GroupZ).nullish(),
  roles: z.array(z.lazy(() => RoleZ)).nullish(),
});

export interface User extends z.infer<typeof UserZ> { }

export type UserF = Omit<User, "group" | "roles">;

export const userForm = formFactoryForModel<UserF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  username: [model.username],
  password: [""],
  emailAddress: [model.emailAddress],
  givenName: [model.givenName],
  familyName: [model.familyName],
  fullName: [model.fullName],
  groupId: [model.groupId],
}));
