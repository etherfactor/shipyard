import { inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalReturn } from '../../utilities/modal/modal.util';

export abstract class InitializeModal<
  TArgs extends Array<any>,
  TClose = any,
  TDismiss = TClose
> {

  private readonly _$activeModal = inject(NgbActiveModal);

  abstract get defaultClose(): TClose;

  abstract get defaultDismiss(): TDismiss;

  abstract initialize(...args: TArgs): void;

  close(result?: TClose): void {
    const use: ModalReturn<TClose, TDismiss> = {
      type: "closed",
      value: result ?? this.defaultClose,
    };
    this._$activeModal.close(use);
  }

  dismiss(result?: TDismiss): void {
    const use: ModalReturn<TClose, TDismiss> = {
      type: "dismissed",
      reason: result ?? this.defaultDismiss,
    };
    this._$activeModal.dismiss(use);
  }
}
