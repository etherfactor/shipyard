import { inject, Injectable } from '@angular/core';
import { AbstractLoggerService } from 'angular-auth-oidc-client';
import { Logger } from '../../utilities/logger/logger.util';

@Injectable({
  providedIn: 'root'
})
export class OidcLoggerService extends AbstractLoggerService {

  private readonly $logger = inject(Logger).forContext("OidcClient")

  override logError(message: string | object, ...args: any[]): void {
    message = sanitize(String(message));
    if (!allowed(message)) return;
    this.$logger.error(String(message), ...args);
  }

  override logWarning(message: string | object, ...args: any[]): void {
    message = sanitize(String(message));
    if (!allowed(message)) return;
    this.$logger.warning(String(message), ...args);
  }

  override logDebug(message: string | object, ...args: any[]): void {
    message = sanitize(String(message));
    if (!allowed(message)) return;
    this.$logger.debug(String(message), ...args);
  }
}

function sanitize(message: string): string {
  if (message.indexOf("storing the accessToken") >= 0) {
    message = message.replace(/'[^']*?'/g, "'<redacted>'");
  }
  if (message.indexOf("AuthResult") >= 0) {
    message = message.replace(/"access_token":\s*"[^"]*?"/g, '"access_token": "<redacted>"');
    message = message.replace(/"id_token":\s*"[^"]*?"/g, '"id_token": "<redacted>"');
    message = message.replace(/"refresh_token":\s*"[^"]*?"/g, '"refresh_token": "<redacted>"');
  }
  return message;
}

function allowed(message: string): boolean {
  if (message.indexOf("matches configured route") >= 0) return false;
  return true;
}
