import { InjectionToken, Provider } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';

bootstrapApplication(
  AppComponent,
  {
    providers: [

    ]
  })
  .catch(err => console.error(err));

function provideSimpleConfig<TConfig>(token: InjectionToken<TConfig>, value: TConfig): Provider {
  return {
    provide: token,
    useValue: value,
  };
}
