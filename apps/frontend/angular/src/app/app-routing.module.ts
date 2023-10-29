import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LobbyComponent } from './shared/lobby/lobby.component';
import { AuthorizationGuard } from './guards/authorization.guard';
import { ProfileResolver } from './resolvers/profile.resolver';

const routes: Routes = [
  // Private module routes
  {
    path: '',
    loadChildren: () => import('./public/public.module').then(mod => mod.PublicModule)
  },
  {
    path: 'account',
    loadChildren: () => import('./account/account.module').then(mod => mod.AccountModule)
  },
  { 
    path: 'onboarding',
    loadChildren: () => import('./onboarding/onboarding.module').then(mod => mod.OnboardingModule)
  },
  {
    path: 'x01',
    loadChildren: () => import('./games/x01/x01.module').then(mod => mod.X01Module)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }