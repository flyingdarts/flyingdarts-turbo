import { NgModule } from '@angular/core';
import { LoadingComponent } from './loading/loading.component';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { AmplifyAuthenticatorModule } from '@aws-amplify/ui-angular';
import { WebcamModule } from 'ngx-webcam';
import { CarouselComponent } from './carousel/carousel.component';
import { LottieModule } from 'ngx-lottie';
import { lottiePlayerFactory } from './lottiePlayerFactory';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DartboardComponent } from './dartboard/dartboard.component';
import { LobbyComponent } from './lobby/lobby.component';

@NgModule({
  declarations: [
    LoadingComponent,
    CarouselComponent,
    NavigationBarComponent,
    DartboardComponent,
    LobbyComponent
  ],
  imports: [
    CommonModule,
    AmplifyAuthenticatorModule,
    HttpClientModule,
    WebcamModule,
    ReactiveFormsModule,
    LottieModule.forRoot({ player: lottiePlayerFactory }),
    RouterModule
  ],
  exports: [
    LoadingComponent,
    CarouselComponent,
    NavigationBarComponent,
    DartboardComponent
  ]
})

export class SharedModule { }
