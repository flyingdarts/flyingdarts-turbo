import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountRootComponent } from './account-root/account-root.component';
import { ProfileComponent } from './profile/profile.component';
import { StatsComponent } from './stats/stats.component';

const routes: Routes = [
  {
    path: "",
    component: AccountRootComponent,
    title: 'Flyingdarts - account',
    children: [
      {
        path: "profile",
        component: ProfileComponent,
        outlet: "account-outlet",
        title: 'Flyingdarts - profile'
      },
      {
        path: "stats",
        component: StatsComponent,
        outlet: "account-outlet",
        title: 'Flyingdarts - stats'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
