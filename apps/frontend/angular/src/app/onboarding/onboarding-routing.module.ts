import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { CameraComponent } from './camera/camera.component';
import { OnboardingRootComponent } from './onboarding-root/onboarding-root.component';
import { ProfileResolver } from '../resolvers/profile.resolver';
import { OnboardingGuard } from './onboarding.guard';

const routes: Routes = [
  {
    path: "",
    component: OnboardingRootComponent,
    children: [
      {
        path: "profile",
        component: ProfileComponent,
        outlet: "onboarding-outlet",
        canActivate: [OnboardingGuard]
      },
      {
        path: "camera",
        component: CameraComponent,
        outlet: "onboarding-outlet"
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnboardingRoutingModule { }
