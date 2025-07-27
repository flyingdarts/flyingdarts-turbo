import { isDevMode, NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { CommonModule } from "@angular/common";
import { provideHttpClient, withInterceptors } from "@angular/common/http";

import { EffectsModule } from "@ngrx/effects";
import { StoreModule } from "@ngrx/store";
import { provideStoreDevtools } from "@ngrx/store-devtools";

import { DyteComponentsModule } from "@dytesdk/angular-ui-kit";
import { AuthressModule } from "@mikepattyn/authress-angular";
import player from "lottie-web";
import { LottieComponent, provideLottieOptions } from "ngx-lottie";

import { environment } from "src/environments/environment";
import { AppComponent } from "./app.component";
import { AppRoutingModule } from "./app-routing.module";
import { NavbarComponent } from "./components/navbar/navbar.component";
import { authInterceptor } from "./interceptors/authentication.interceptor";
import { AppStateStoreModule } from "./state/app";
import { AppEffects } from "./state/app/app.effects";
import { GameStateStoreModule } from "./state/game";
import { FriendsEffects } from "./state/friends/friends.effects";
import { FriendsStoreModule } from "./state/friends/friends-store.module";

@NgModule({
  declarations: [AppComponent],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    NavbarComponent,
    DyteComponentsModule,
    StoreModule.forRoot(),
    AppStateStoreModule,
    FriendsStoreModule,
    GameStateStoreModule,
    EffectsModule.forRoot([AppEffects, FriendsEffects]),
    LottieComponent,
    AuthressModule.forRoot({
      authressApiUrl: environment.authressLoginUrl,
      applicationId: environment.authressAppId,
    }),
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
    provideLottieOptions({ player: () => player }),
    provideStoreDevtools({
      maxAge: 25, // Retains last 25 states
      logOnly: !isDevMode(), // Restrict extension to log-only mode
      autoPause: true, // Pauses recording actions and state changes when the extension window is not open
      trace: false, //  If set to true, will include stack trace for every dispatched action, so you can see it in trace tab jumping directly to that part of code
      traceLimit: 75, // maximum stack trace frames to be stored (in case trace option was provided as true)
      connectInZone: true, // If set to true, the connection is established within the Angular zone
    }),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
