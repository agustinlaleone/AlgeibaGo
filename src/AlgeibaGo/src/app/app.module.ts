import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RedirectComponent } from './redirect/redirect.component';
import { ErrorsHandlerComponent } from './errors-handler/errors-handler.component';
import { HomeComponent } from './home/home.component';
import { HttpClientModule, HttpClient } from '@angular/common/http'; 
import { AppConfigService } from './service/AppConfigService';
import { DeviceDetectorModule } from 'ngx-device-detector';
import { FooterComponent } from './footer/footer.component';
import { ServererrorComponent } from './errors-handler/servererror/servererror.component';

const appInitializerFn = (appConfig: AppConfigService) => {
  return () => {
      return appConfig.loadAppConfig();
  }
};

@NgModule({
  declarations: [
    AppComponent,
    RedirectComponent,
    ErrorsHandlerComponent,
    HomeComponent,
    FooterComponent,
    ServererrorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    DeviceDetectorModule.forRoot()
  ],
  providers: [
    {
      provide: ErrorHandler,
      useClass: ErrorsHandlerComponent
    },
    AppConfigService,
    {
        provide: APP_INITIALIZER,
        useFactory: appInitializerFn,
        multi: true,
        deps: [AppConfigService]
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }