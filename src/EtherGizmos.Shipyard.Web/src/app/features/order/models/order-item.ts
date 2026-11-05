import z from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { AppValidators, formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { OrderZ } from "./order";

export const OrderItemZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  orderId: z.number().int(),
  get order() {
    return z.lazy(() => OrderZ);
  },
  description: z.string(),
  quantity: z.number(),
  quantityUnits: z.string().nullish(),
  unitPrice: z.number().nullish(),
  totalPrice: z.number().nullish(),
});

export interface OrderItem extends z.infer<typeof OrderItemZ> { }

export type OrderItemF = Omit<OrderItem, "order">;

export const orderItemForm = formFactoryForModel<OrderItemF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  orderId: [model.orderId, AppValidators.required],
  description: [model.description, AppValidators.required],
  quantity: [model.quantity],
  quantityUnits: [model.quantityUnits],
  unitPrice: [model.unitPrice],
  totalPrice: [model.totalPrice],
}));
