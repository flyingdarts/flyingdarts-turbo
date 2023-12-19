import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueComponent } from './queue.component';
import { SharedModule } from '../../shared/shared.module';
import { ComponentStore } from '@ngrx/component-store';
import { QueueRootComponent } from './queue-root.component';
import { FormsModule } from '@angular/forms';
import { LottieModule } from 'ngx-lottie';
import { lottiePlayerFactory } from 'src/app/shared/lottiePlayerFactory';
import { QueueRoutingModule } from './queue-routing.module';

@NgModule({
  declarations: [
    QueueRootComponent,
    QueueComponent
  ],
  imports: [
    CommonModule,
    QueueRoutingModule,
    SharedModule,
    FormsModule, // Add FormsModule to the imports array
    LottieModule.forRoot({ player: lottiePlayerFactory }),
  ],
  providers: [ComponentStore]
})
export class QueueModule { }
