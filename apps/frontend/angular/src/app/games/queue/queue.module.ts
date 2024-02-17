import { InjectionToken, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueComponent } from './queue.component';
import { SharedModule } from '../../shared/shared.module';
import { ComponentStore } from '@ngrx/component-store';
import { QueueRootComponent } from './queue-root.component';
import { FormsModule } from '@angular/forms';
import { QueueRoutingModule } from './queue-routing.module';
import { LoadingComponent } from 'src/app/shared/loading/loading.component';
import { LottieComponent, provideLottieOptions } from 'ngx-lottie';
import { lottiePlayerFactory } from 'src/app/shared/lottiePlayerFactory';

@NgModule({
  declarations: [
    QueueRootComponent, 
    QueueComponent,
  ],
  imports: [
    QueueRoutingModule,
    CommonModule,
    SharedModule,
    LottieComponent,
    LoadingComponent,
    FormsModule,
  ],
  providers: [
    ComponentStore, 
    ],
})
export class QueueModule { }
