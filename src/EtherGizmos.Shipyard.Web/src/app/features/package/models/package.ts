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
  trackingUpdates: z.array(TrackingUpdateZ),
});

export interface PackageF extends z.infer<typeof PackageZ> { }

export type Package = Omit<PackageF, "carrier" | "trackingUpdates">;

export const packageForm = formFactoryForModel<Package>(($form, model) => ({
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
