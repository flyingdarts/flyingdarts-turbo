import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PublicRoutingModule } from './public-routing.module';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';
import { TermsOfServiceComponent } from './terms-of-service/terms-of-service.component';
import { SharedModule } from '../shared/shared.module';
import { LoginComponent } from './login/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { LobbyComponent } from './lobby/lobby.component';

@NgModule({
  declarations: [
    LeaderboardComponent,
    PrivacyPolicyComponent, 
    TermsOfServiceComponent,
    LoginComponent,
    HomeComponent,
    LobbyComponent
  ],
  imports: [
    CommonModule,
    PublicRoutingModule,
    SharedModule,
    ReactiveFormsModule,
  ]
})
export class PublicModule { }
