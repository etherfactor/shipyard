import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { OrderFulfillmentZ } from "./order-fulfillment";
import { OrderItemZ } from "./order-item";

export const OrderFulfillmentItemZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  orderFulfillmentId: z.number().int(),
  get orderFulfillment() {
    return z.lazy(() => OrderFulfillmentZ);
  },
  orderItemId: z.number().int(),
  get orderItem() {
    return z.lazy(() => OrderItemZ);
  },
  expectedQuantity: z.number(),
  receivedQuantity: z.number().nullish(),
});

export interface OrderFulfillmentItem extends z.infer<typeof OrderFulfillmentItemZ> { }

export type OrderFulfillmentItemF = Omit<OrderFulfillmentItem, "orderFulfillment" | "orderItem">;

export const orderFulfillmentItemForm = formFactoryForModel<OrderFulfillmentItemF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  orderFulfillmentId: [model.orderFulfillmentId, AppValidators.required],
  orderItemId: [model.orderItemId, AppValidators.required],
  expectedQuantity: [model.expectedQuantity],
  receivedQuantity: [model.receivedQuantity],
}));
