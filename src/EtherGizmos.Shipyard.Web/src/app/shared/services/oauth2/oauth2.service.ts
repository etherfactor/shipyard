import { Injectable, Signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
  useFactory: () => { throw new Error("Abstract service; cannot instantiate directly"); }
})
export abstract class OAuth2Service {

  abstract readonly onReady: Promise<void>;
  abstract readonly isReady$$: Signal<boolean>;
  abstract readonly accessToken$$: Signal<string>;
  abstract readonly idToken$$: Signal<string>;
  abstract readonly idTokenData$$: Signal<Record<string, any>>;

  abstract login(): void;
  abstract logout(): void;
}
