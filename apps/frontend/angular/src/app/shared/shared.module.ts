import { NgModule } from '@angular/core';
import { LoadingComponent } from './loading/loading.component';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { AmplifyAuthenticatorModule } from '@aws-amplify/ui-angular';
import { CarouselComponent } from './carousel/carousel.component';
import { lottiePlayerFactory } from './lottiePlayerFactory';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DartboardComponent } from './dartboard/dartboard.component';
import { LobbyComponent } from './lobby/lobby.component';
import { LottieModule } from 'ngx-lottie';

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
