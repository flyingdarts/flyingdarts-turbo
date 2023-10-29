import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OnboardingRoutingModule } from './onboarding-routing.module';
import { CameraComponent } from './camera/camera.component';
import { ProfileComponent } from './profile/profile.component';
import { ReactiveFormsModule } from '@angular/forms';
import { OnboardingRootComponent } from './onboarding-root/onboarding-root.component';
import { SharedModule } from '../shared/shared.module';
import { UserProfileApiService } from '../services/user-profile-api.service';


@NgModule({
  declarations: [
    ProfileComponent,
    CameraComponent,
    OnboardingRootComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    OnboardingRoutingModule,
    SharedModule
  ],
  providers: [
    UserProfileApiService,
  ]
})
export class OnboardingModule { }
