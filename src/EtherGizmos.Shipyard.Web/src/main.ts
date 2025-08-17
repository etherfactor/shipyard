/// <reference types="@angular/localize" />

import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { InjectionToken, Provider } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { APP_CONFIG, fetchConfig } from './app/shared/utilities/config/config.util';
import { provideODataClient } from './app/shared/utilities/odata/odata.util';

(async () => {
  const config = await fetchConfig();

  bootstrapApplication(
    AppComponent,
    {
      providers: [
        provideSimpleConfig(APP_CONFIG, config),
        provideRouter(APP_ROUTES),
        provideHttpClient(withFetch(), withInterceptors([])),
        provideODataClient(),
        provideMonacoEditor({
          baseUrl: window.location.origin + "/assets/monaco/min/vs",
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
