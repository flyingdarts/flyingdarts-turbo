import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountRootComponent } from './account-root/account-root.component';
import { AuthorizationGuard } from '../guards/authorization.guard';
import { ProfileComponent } from './profile/profile.component';
import { SettingsComponent } from './settings/settings.component';
import { ProfileDetailsResolver } from '../resolvers/profile.resolver';
import { StatsComponent } from './stats/stats.component';

const routes: Routes = [
  {
    path: "",
    component: AccountRootComponent,
    canActivate: [AuthorizationGuard],
    resolve: { 
      userProfileDetails: ProfileDetailsResolver
    },
    children: [
      {
        path: "profile",
        component: ProfileComponent,
        outlet: "account-outlet"
      },
      {
        path: "settings",
        component: SettingsComponent,
        outlet: "account-outlet"
      },
      {
        path: "stats",
        component: StatsComponent,
        outlet: "account-outlet"
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
