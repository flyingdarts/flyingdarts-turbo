import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { AccountRootComponent } from './account-root/account-root.component';
import { ProfileComponent } from './profile/profile.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { StatsComponent } from './stats/stats.component';
import { LoadingComponent } from '../shared/loading/loading.component';
import { LottieComponent } from 'ngx-lottie';


@NgModule({
  declarations: [
    AccountRootComponent,
    ProfileComponent,
    StatsComponent
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    LoadingComponent,
    LottieComponent
  ]
})
export class AccountModule { }
