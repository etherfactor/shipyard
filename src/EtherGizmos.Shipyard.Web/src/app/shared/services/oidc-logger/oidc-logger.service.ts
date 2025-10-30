import { inject, Injectable } from '@angular/core';
import { AbstractLoggerService } from 'angular-auth-oidc-client';
import { Logger } from '../../utilities/logger/logger.util';

@Injectable({
  providedIn: 'root'
})
export class OidcLoggerService extends AbstractLoggerService {

  private readonly $logger = inject(Logger).forContext("OidcClient")

  override logError(message: string | object, ...args: any[]): void {
    this.$logger.error(String(message), ...args);
  }

  override logWarning(message: string | object, ...args: any[]): void {
    this.$logger.warning(String(message), ...args);
  }

  override logDebug(message: string | object, ...args: any[]): void {
    this.$logger.debug(String(message), ...args);
  }
}
