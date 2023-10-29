import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { X01RoutingModule } from './x01-routing.module';
import { X01Component } from './x01.component';
import { SharedModule } from '../../shared/shared.module';
import { ComponentStore } from '@ngrx/component-store';
import { X01RootComponent } from './x01-root.component';
import { FormsModule } from '@angular/forms';
import { LottieModule } from 'ngx-lottie';
import { lottiePlayerFactory } from 'src/app/shared/lottiePlayerFactory';

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
    LottieModule.forRoot({ player: lottiePlayerFactory }),
  ],
  providers: [ComponentStore]
})
export class X01Module { }
