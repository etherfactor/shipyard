import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";
import { formFactoryForModel } from "../../../shared/utilities/form/form.util";
import { CarrierZ } from "./carrier";
import { carrierExecutionArtifactForm, CarrierExecutionArtifactZ } from "./carrier-execution-artifact";
import { ExecutionStatusType } from "./execution-status-type";

export const CarrierExecutionZ = z.object({
  id: z.number().int(),
  createdAt: DateTimeZ,
  modifiedAt: DateTimeZ,
  carrierId: z.number().int(),
  carrier: z.lazy(() => CarrierZ).nullish(),
  startedAt: DateTimeZ.nullish(),
  completedAt: DateTimeZ.nullish(),
  executionStatusType: z.nativeEnum(ExecutionStatusType),
  stepCount: z.number().int(),
  failureStepIndex: z.number().int().nullish(),
  artifacts: z.array(CarrierExecutionArtifactZ),
});

export interface CarrierExecution extends z.infer<typeof CarrierExecutionZ> { }

export type CarrierExecutionF = Omit<CarrierExecution, "carrier">;

export const carrierExecutionForm = formFactoryForModel<CarrierExecutionF>(($form, model) => ({
  id: [model.id],
  createdAt: [model.createdAt],
  modifiedAt: [model.modifiedAt],
  carrierId: [model.carrierId],
  startedAt: [model.startedAt],
  completedAt: [model.completedAt],
  executionStatusType: [model.executionStatusType],
  stepCount: [model.stepCount],
  failureStepIndex: [model.failureStepIndex],
  artifacts: $form.nonNullable.array(model.artifacts.map(item => carrierExecutionArtifactForm($form, item))),
}));
