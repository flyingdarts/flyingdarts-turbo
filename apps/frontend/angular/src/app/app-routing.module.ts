import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

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
  },
  {
    path: 'queue',
    loadChildren: () => import('./games/queue/queue.module').then(mod => mod.QueueModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }