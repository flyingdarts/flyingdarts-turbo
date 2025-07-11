import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameContainerComponent } from './features/game/container/game.container';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { UserProfileResolver } from './resolvers/user-profile.resolver';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        resolve: {
          profileResolved: UserProfileResolver,
        },
        loadChildren: () =>
          import('./features/home/home.module').then((m) => m.HomeModule),
      },
      {
        path: 'game/:meetingId',
        component: GameContainerComponent,
      },
    ],
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }, // Redirect unknown paths to home
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
