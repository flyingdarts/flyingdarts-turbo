import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { TermsOfServiceComponent } from './terms-of-service/terms-of-service.component';
import { LoginComponent } from './login/login.component';
import { LobbyComponent } from './lobby/lobby.component';
import { canActivateLobby } from '../guards/lobby.guard';
import { canActivateLogin } from '../guards/login.guard';
import { userProfileResolver } from '../resolvers/user-profile.resolver';

const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    canActivate: [canActivateLogin]
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [canActivateLogin],
    title: 'Flyingdarts - login',
  },
  {
    path: 'lobby',
    component: LobbyComponent,
    canActivate: [canActivateLobby],
    resolve: {userProfile: userProfileResolver},
    title: 'Flyingdarts - lobby'
  },
  {
    path: 'terms-of-service',
    component: TermsOfServiceComponent,
    title: 'Flyingdarts - terms of service'
  },
  {
    path: 'privacy-policy',
    component: PrivacyPolicyComponent,
    title: 'Flyingdarts - privacy policy'
  },
  {
    path: 'leaderboard',
    component: LeaderboardComponent,
    title: 'Flyingdarts - leaderboard'
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
