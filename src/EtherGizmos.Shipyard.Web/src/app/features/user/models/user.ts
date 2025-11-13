import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";

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
});

export interface User extends z.infer<typeof UserZ> { }

export type UserF = Omit<User, "">;

export const userForm = formFactoryForModel<User>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  username: [model.username],
  password: [model.password],
  emailAddress: [model.emailAddress],
  givenName: [model.givenName],
  familyName: [model.familyName],
  fullName: [model.fullName],
}));
