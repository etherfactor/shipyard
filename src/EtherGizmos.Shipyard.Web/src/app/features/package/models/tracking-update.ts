import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { StatusType } from "./status-type";

export const TrackingUpdateZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  occurredAt: DateTimeZ,
  statusType: z.nativeEnum(StatusType),
  location: z.string().nullish(),
  description: z.string().nullish(),
});

export interface TrackingUpdate extends z.infer<typeof TrackingUpdateZ> { }

export const trackingUpdateForm = formFactoryForModel<TrackingUpdate>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  occurredAt: [model.occurredAt],
  statusType: [model.statusType],
  location: [model.location],
  description: [model.description],
}));
