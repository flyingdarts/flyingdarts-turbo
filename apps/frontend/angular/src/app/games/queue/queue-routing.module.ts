import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QueueComponent } from './queue.component';
import { QueueRootComponent } from './queue-root.component';

const routes: Routes = [
  {
    path: "",
    component: QueueRootComponent,
    //canActivate: [AuthorizationGuard],
    children: [
      {
        path: ':gameSettingsType',
        component: QueueComponent
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QueueRoutingModule { }