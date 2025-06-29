import { NgbOffcanvas } from "@ng-bootstrap/ng-bootstrap";
import { InitializeOffcanvas } from "../../components/initialize-offcanvas/initialize-offcanvas.component";

export async function openOffcanvas<
  TComponent extends InitializeOffcanvas<any, any, any>,
  TClose = ExtractClose<TComponent>,
  TDismiss = ExtractDismiss<TComponent>
>(
  offcanvas: NgbOffcanvas,
  component: new (...args: any[]) => TComponent,
  ...args: ExtractArgs<TComponent>
): Promise<OffcanvasReturn<TClose, TDismiss>> {
  const ref = offcanvas.open(component, { panelClass: "sidebar" });
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
  TComponent extends InitializeOffcanvas<infer U, any, any>
  ? U : never;

type ExtractClose<TComponent> =
  TComponent extends InitializeOffcanvas<any, infer U, any>
  ? U : never;

type ExtractDismiss<TComponent> =
  TComponent extends InitializeOffcanvas<any, any, infer U>
  ? U : never;

export type OffcanvasReturn<TClose, TDismiss> =
  | { type: "closed"; value: TClose }
  | { type: "dismissed"; reason: TDismiss };
