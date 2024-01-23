import { NgModule } from '@angular/core';
import { LoadingComponent } from './loading/loading.component';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { CarouselComponent } from './carousel/carousel.component';
import { lottiePlayerFactory } from './lottiePlayerFactory';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DartboardComponent } from './dartboard/dartboard.component';
import { LottieModule } from 'ngx-lottie';

@NgModule({
  declarations: [
    LoadingComponent,
    CarouselComponent,
    NavigationBarComponent,
    DartboardComponent
  ],
  imports: [
    CommonModule,
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
