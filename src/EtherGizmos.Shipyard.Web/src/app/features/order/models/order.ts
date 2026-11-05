import z from "zod";
import { DateZ } from "../../../shared/types/date/date";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { OrderFulfillmentZ } from "./order-fulfillment";
import { OrderItemZ } from "./order-item";

export const OrderZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  source: z.string().nullish(),
  orderNumber: z.string().nullish(),
  orderedOn: DateZ.nullish(),
  totalPrice: z.number().nullish(),
  orderUrl: z.string().nullish(),
  expectedShipStartOn: DateZ.nullish(),
  expectedShipEndOn: DateZ.nullish(),
  get orderItems() {
    return z.lazy(() => OrderItemZ).array();
  },
  get orderFulfillments() {
    return z.lazy(() => OrderFulfillmentZ).array();
  },
});

export interface Order extends z.infer<typeof OrderZ> { }

export type OrderF = Omit<Order, "orderItems" | "orderFulfillments">;

export const orderForm = formFactoryForModel<OrderF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  source: [model.source],
  orderNumber: [model.orderNumber],
  orderedOn: [model.orderedOn],
  totalPrice: [model.totalPrice],
  orderUrl: [model.orderUrl],
  expectedShipStartOn: [model.expectedShipStartOn],
  expectedShipEndOn: [model.expectedShipEndOn],
}));
