import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AmplifyAuthService } from './services/amplify-auth.service';
import { ApiService } from './services/room-api.service';
import { JitsiService } from './services/jitsi.service';
import { LoadingService } from './services/loading.service';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
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
import { EffectsModule } from '@ngrx/effects';
import { AuthEffects } from './auth.effects';
import { StoreModule } from '@ngrx/store';

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
    EffectsModule.forRoot([AuthEffects]),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true
    },
    WebSocketService,
    WebSocketMessageService,
    ApiService,
    LoadingService,
    AmplifyAuthService,
    JitsiService,
    WebcamService,
    UserProfileStateService,
    UserProfileApiService,
    X01ApiService,
    provideComponentStore(AppStore),
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
