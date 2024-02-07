import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { JitsiService } from './services/jitsi.service';
import { LoadingService } from './services/loading.service';
import { WebcamService } from './services/webcam.service';
import { UserProfileApiService } from './services/user-profile-api.service';
import { UserProfileStateService } from './services/user-profile-state.service';
import { SharedModule } from './shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { X01ApiService } from './services/x01-api.service';
import { WebSocketService } from './infrastructure/websocket/websocket.service';
import { WebSocketMessageService } from './infrastructure/websocket/websocket-message.service';
import { provideComponentStore } from '@ngrx/component-store';
import { AppStore } from './app.store';
import { StoreModule } from '@ngrx/store';
import { ApiClient } from './services/api.client';
import { PreferedX01SettingsService } from './services/prefered-x01-settings.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthressService } from './services/authress_service';
import { StatsApiService } from './account/stats/stats-api.service';
import { StatsStateService } from './account/stats/stats-state.service';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    StoreModule.forRoot({}),
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [
    WebSocketService,
    WebSocketMessageService,
    LoadingService,
    JitsiService,
    WebcamService,
    UserProfileStateService,
    UserProfileApiService,
    X01ApiService,
    provideComponentStore(AppStore),
    ApiClient,
    PreferedX01SettingsService,
    AuthressService,
    StatsApiService,
    StatsStateService
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
