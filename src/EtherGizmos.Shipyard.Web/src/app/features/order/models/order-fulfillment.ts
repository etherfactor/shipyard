import z from "zod";
import { DateZ } from "../../../shared/types/date/date";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { GuidZ } from "../../../shared/types/guid/guid";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { UserZ } from "../../user/models/user";
import { OrderZ } from "./order";
import { OrderFulfillmentItemZ } from "./order-fulfillment-item";
import { OrderFulfillmentStatusType } from "./order-fulfillment-status-type";

export const OrderFulfillmentZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  orderId: z.number().int(),
  get order() {
    return z.lazy(() => OrderZ);
  },
  sender: z.string().nullish(),
  orderFulfillmentStatusType: z.enum(OrderFulfillmentStatusType),
  shippedOn: DateZ.nullish(),
  receivedAt: DateTimeZ.nullish(),
  receivedByUserId: GuidZ.nullish(),
  receivedByUser: z.lazy(() => UserZ),
  resolvedAt: DateTimeZ.nullish(),
  resolvedByUserId: GuidZ.nullish(),
  resolvedByUser: z.lazy(() => UserZ),
  resolvedReason: z.string().nullish(),
  get orderFulfillmentItems() {
    return z.lazy(() => OrderFulfillmentItemZ).array().nullish();
  },
});

export interface OrderFulfillment extends z.infer<typeof OrderFulfillmentZ> { }

export type OrderFulfillmentF = Omit<OrderFulfillment, "order" | "receivedByUser" | "resolvedByUser" | "orderFulfillmentItems">;

export const orderFulfillmentForm = formFactoryForModel<OrderFulfillmentF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  orderId: [model.orderId, AppValidators.required],
  sender: [model.sender],
  orderFulfillmentStatusType: [model.orderFulfillmentStatusType, AppValidators.required],
  shippedOn: [model.shippedOn],
  receivedAt: [model.receivedAt],
  receivedByUserId: [model.receivedByUserId],
  resolvedAt: [model.resolvedAt],
  resolvedByUserId: [model.resolvedByUserId],
  resolvedReason: [model.resolvedReason],
}));
