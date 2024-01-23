import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { TermsOfServiceComponent } from './terms-of-service/terms-of-service.component';
import { LoginComponent } from './login/login.component';
import { LobbyComponent } from './lobby/lobby.component';
import { AuthorizationGuard } from '../guards/authorization.guard';
import { ProfileDetailsResolver } from '../resolvers/profile.resolver';

const routes: Routes = [
  {
    path: '',
    component: LoginComponent
  },
  {
    path: 'lobby',
    canActivate: [AuthorizationGuard],
    component: LobbyComponent,
    resolve: {
      profileDetails: ProfileDetailsResolver
    }
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'terms-of-service',
    component: TermsOfServiceComponent
  },
  {
    path: 'privacy-policy',
    component: PrivacyPolicyComponent
  },
  {
    path: 'leaderboard',
    component: LeaderboardComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
