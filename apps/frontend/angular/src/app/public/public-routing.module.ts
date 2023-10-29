import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { TermsOfServiceComponent } from './terms-of-service/terms-of-service.component';
import { LoginComponent } from './login/login.component';
import { LoginGuard } from '../guards/login.guard';
import { LobbyComponent } from '../shared/lobby/lobby.component';
import { AuthorizationGuard } from '../guards/authorization.guard';
import { ProfileResolver } from '../resolvers/profile.resolver';

const routes: Routes = [
  {
    path: '',
    resolve: { profile: ProfileResolver },
    component: LobbyComponent
  },
  {
    path: 'lobby',
    canActivate: [AuthorizationGuard],
    component: LobbyComponent
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [LoginGuard],
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
