// feature-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { X01GameSettingsGuard } from 'src/app/guards/x01-game-settings.guard';
import { HomeLobbyComponent } from './lobby/home-lobby.component';
import { HomeRootComponent } from './root/home-root.component';
import { HomeSettingsComponent } from './settings/home-settings.component';

const routes: Routes = [
  {
    path: '',
    component: HomeRootComponent,
    children: [
      {
        path: '',
        component: HomeLobbyComponent,
        canActivate: [X01GameSettingsGuard],
      },
      {
        path: 'settings',
        component: HomeSettingsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}
