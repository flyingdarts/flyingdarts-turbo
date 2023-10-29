import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { CameraComponent } from './camera/camera.component';
import { OnboardingRootComponent } from './onboarding-root/onboarding-root.component';
import { ProfileResolver } from '../resolvers/profile.resolver';

const routes: Routes = [
  {
    path: "",
    component: OnboardingRootComponent,
    children: [
      {
        path: "profile",
        component: ProfileComponent,
        outlet: "onboarding-outlet",
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
