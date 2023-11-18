import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StatsRootComponent } from './stats-root.component';
import { X01StatsComponent } from './x01-stats/x01-stats.component';

const routes: Routes = [
  {
    path: '',
    component: StatsRootComponent,
    children: [
      {
        path: 'x01',
        component: X01StatsComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StatsRoutingModule { }
