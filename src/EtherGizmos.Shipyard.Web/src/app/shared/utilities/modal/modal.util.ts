import { NgbModal, NgbModalOptions } from "@ng-bootstrap/ng-bootstrap";
import { InitializeModal } from "../../components/initialize-modal/initialize-modal.component";

export async function openModal<
  TComponent extends InitializeModal<any, any, any>,
  TClose = ExtractClose<TComponent>,
  TDismiss = ExtractDismiss<TComponent>
>(
  options: ModalInput,
  component: new (...args: any[]) => TComponent,
  ...args: ExtractArgs<TComponent>
): Promise<ModalReturn<TClose, TDismiss>> {
  const useOptions: NgbModalOptions = { size: "md", centered: true, backdrop: "static", keyboard: false, ...options.options };
  const ref = options.modal.open(component, useOptions);
  const compInstance = <TComponent>ref.componentInstance;

  compInstance.initialize(...args as any[]);

  try {
    return await ref.result;
  } catch {
    return {
      type: "dismissed",
      reason: compInstance.defaultDismiss,
    };
  }
}

type ExtractArgs<TComponent> =
  TComponent extends InitializeModal<infer U, any, any>
  ? U : never;

type ExtractClose<TComponent> =
  TComponent extends InitializeModal<any, infer U, any>
  ? U : never;

type ExtractDismiss<TComponent> =
  TComponent extends InitializeModal<any, any, infer U>
  ? U : never;

export type ModalReturn<TClose, TDismiss> =
  | { type: "closed"; value: TClose }
  | { type: "dismissed"; reason: TDismiss };

interface ModalInput {
  modal: NgbModal,
  options?: Partial<NgbModalOptions>,
}
