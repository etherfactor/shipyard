/// <reference types="@angular/localize" />

import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { InjectionToken, isDevMode, Provider } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, TitleStrategy } from '@angular/router';
import { provideServiceWorker } from '@angular/service-worker';
import { buildUrl } from '@ethergizmos/odata-fluent-client/dist/src/utils/http';
import { AbstractSecurityStorage, authInterceptor, DefaultLocalStorageService, LogLevel, provideAuth } from 'angular-auth-oidc-client';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { provideOAuth2Service } from './app/shared/services/oauth2/oauth2.service.concrete';
import { TitleStrategyService } from './app/shared/services/title-strategy/title-strategy.service';
import { APP_CONFIG, fetchConfig } from './app/shared/utilities/config/config.util';
import { ConsoleSink, SourceContextEnricher } from './app/shared/utilities/logger/logger.extra.util';
import { LoggerConfiguration, provideLogger } from './app/shared/utilities/logger/logger.util';
import { provideODataClient } from './app/shared/utilities/odata/odata.util';

(async () => {
  const config = await fetchConfig();

  bootstrapApplication(
    AppComponent,
    {
      providers: [
        provideSimpleConfig(APP_CONFIG, config),
        provideRouter(APP_ROUTES),
        provideHttpClient(
          withFetch(),
          withInterceptors([
            authInterceptor(),
          ])
        ),
        provideODataClient(),
        provideLogger(
          [ConsoleSink, SourceContextEnricher] as const,
          (consoleSink, sourceContextEnricher) =>
            new LoggerConfiguration()
              .writeTo.sink(consoleSink)
              .enrich.with(sourceContextEnricher)
              .createLogger()
        ),
        provideOAuth2Service(),
        provideAuth({
          config: {
            configId: "webui",
            authority: config.oauth.authority,
            redirectUrl: buildUrl(window.location.href, "/login/callback"),
            postLoginRoute: "login/callback",
            clientId: config.oauth.clientId,
            scope: config.oauth.scope,
            responseType: "code",
            silentRenew: true,
            useRefreshToken: true,
            autoUserInfo: false,
            secureRoutes: [config.resourceServer],
            logLevel: LogLevel.Warn,
            silentRenewTimeoutInSeconds: 30,
            tokenRefreshInSeconds: 15,
            ignoreNonceAfterRefresh: true,
            disableIatOffsetValidation: true,
            disableIdTokenValidation: true,
            renewTimeBeforeTokenExpiresInSeconds: 300,
            customParamsAuthRequest: {
              prompt: "login",
            },
          },
        }),
        { provide: AbstractSecurityStorage, useClass: DefaultLocalStorageService },
        { provide: TitleStrategy, useClass: TitleStrategyService },
        provideMonacoEditor({
          baseUrl: window.location.origin + "/assets/monaco/min/vs",
          onMonacoLoad: () => {
            const monaco: typeof import("monaco-editor") = (window as any).monaco;

            monaco.editor.onDidCreateModel((m) => {
              console.log('[model created]', m.uri.toString(), 'lang=', m.getLanguageId());
            });

            monaco.editor.onWillDisposeModel((m) => {
              console.log('[model disposed]', m.uri.toString());
            });

            const libSource = `// shipyard-scripting.d.ts
declare global {
  type Timestamp = Instant | string;

  interface Instant {
    toString(): string;
    plus(delta: { hours?: number; minutes?: number; seconds?: number }): Instant;
  }

  interface TrackingEvent {
    at: Timestamp;
    description?: string;
    location?: string | null;
    statusCode?: string | null;
    fingerprint?: string | null;
  }

  interface TransformNode {
    selectAll(selector: string): TransformNode[];
    selectOne(selector: string): TransformNode | null;
    text(): string;
    html(): string;
    attribute(name: string): string | null;
    hasAttribute(name: string): boolean;
    attributes(): Readonly<Record<string, string>>;
    hasClass(name: string): boolean;
    classes(): string[];
    parent(): TransformNode | null;
    nextUntil(stopSelector: string): TransformNode[];
  }

  function selectAll(selector: string): TransformNode[];
  function selectOne(selector: string): TransformNode | null;

  function parseDate(text: string, format: string, tz?: string): Instant;
  function regexMatch(text: string, pattern: string): string[] | null;
  function normalize(text: string): string;
  function hash(text: string): string;

  function recordEvent(event: TrackingEvent): void;
  function setEta(at: Timestamp): void;
}
export {};
`;
            const libUri = "file:///public/shipyard-scripting.v1.d.ts";

            monaco.languages.typescript.javascriptDefaults.addExtraLib(libSource, libUri);
          },
        }),
        provideServiceWorker("ngsw-worker.js", {
          enabled: !isDevMode(),
          registrationStrategy: "registerWhenStable:30000",
        }),
      ]
    })
    .catch(err => console.error(err));
})();

function provideSimpleConfig<TConfig>(token: InjectionToken<TConfig>, value: TConfig): Provider {
  return {
    provide: token,
    useValue: value,
  };
}
