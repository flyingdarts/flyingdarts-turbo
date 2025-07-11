import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';

import { CommonModule } from '@angular/common';
import { AuthressModule, LoginClient } from '@mikepattyn/authress-angular';

import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';

@NgModule({
  declarations: [AppComponent],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,

    AuthressModule.forRoot({
      authressApiUrl: environment.authressLoginUrl,
      applicationId: environment.authressAppId,
    }),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

export class AuthressService {
  constructor(private readonly LoginClient: LoginClient) {}
}
