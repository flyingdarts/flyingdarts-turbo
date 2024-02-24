import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { X01RoutingModule } from './x01-routing.module';
import { X01Component } from './x01.component';
import { SharedModule } from '../../shared/shared.module';
import { ComponentStore } from '@ngrx/component-store';
import { X01RootComponent } from './x01-root.component';
import { FormsModule } from '@angular/forms';
import { LoadingComponent } from 'src/app/shared/loading/loading.component';
import { LottieComponent, provideLottieOptions } from 'ngx-lottie';
import { lottiePlayerFactory } from 'src/app/shared/lottiePlayerFactory';
import { VideoComponent } from 'src/app/shared/video/video.component';

@NgModule({
  declarations: [
    X01RootComponent,
    X01Component
  ],
  imports: [
    CommonModule,
    X01RoutingModule,
    SharedModule,
    FormsModule, // Add FormsModule to the imports array
    LottieComponent,
    LoadingComponent,
    VideoComponent,
  ],
  providers: [ComponentStore, 
  provideLottieOptions(lottiePlayerFactory())]
})
export class X01Module { }
