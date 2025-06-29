import { InjectionToken, Provider } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';

bootstrapApplication(
  AppComponent,
  {
    providers: [
      provideRouter(APP_ROUTES),
    ]
  })
  .catch(err => console.error(err));

function provideSimpleConfig<TConfig>(token: InjectionToken<TConfig>, value: TConfig): Provider {
  return {
    provide: token,
    useValue: value,
  };
}
