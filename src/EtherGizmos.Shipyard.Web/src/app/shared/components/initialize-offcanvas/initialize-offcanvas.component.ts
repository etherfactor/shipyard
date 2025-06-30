import { inject } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { OffcanvasReturn } from '../../utilities/offcanvas/offcanvas.util';

export abstract class InitializeOffcanvas<
  TArgs extends Array<any>,
  TClose = any,
  TDismiss = TClose
> {

  private readonly _$activeOffcanvas = inject(NgbActiveOffcanvas);

  abstract get defaultDismiss(): TDismiss;

  abstract initialize(...args: TArgs): void;

  close(result: TClose): void {
    const use: OffcanvasReturn<TClose, TDismiss> = {
      type: "closed",
      value: result,
    };
    this._$activeOffcanvas.close(use);
  }

  dismiss(result: TDismiss): void {
    const use: OffcanvasReturn<TClose, TDismiss> = {
      type: "dismissed",
      reason: result,
    };
    this._$activeOffcanvas.dismiss(use);
  }
}
