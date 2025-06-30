import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { CarrierZ } from "../../carrier/models/carrier";
import { StatusType } from "./status-type";
import { TrackingUpdateZ } from "./tracking-update";

export const PackageZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  carrierId: z.number(),
  carrier: CarrierZ.nullish(),
  trackingNumber: z.string(),
  contents: z.string(),
  estimatedDeliveryAt: DateTimeZ.nullish(),
  lastPollAt: DateTimeZ,
  nextPollAt: DateTimeZ,
  lastStatusType: z.nativeEnum(StatusType),
  isDelivered: z.boolean(),
  trackingUpdates: z.array(TrackingUpdateZ).nullish(),
});

export interface Package extends z.infer<typeof PackageZ> { }

export type PackageF = Omit<Package, "carrier" | "trackingUpdates">;

export const packageForm = formFactoryForModel<PackageF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  carrierId: [model.carrierId, AppValidators.required],
  trackingNumber: [model.trackingNumber, AppValidators.required],
  contents: [model.contents],
  estimatedDeliveryAt: [model.estimatedDeliveryAt],
  lastPollAt: [model.lastPollAt],
  nextPollAt: [model.nextPollAt],
  lastStatusType: [model.lastStatusType],
  isDelivered: [model.isDelivered],
}));
