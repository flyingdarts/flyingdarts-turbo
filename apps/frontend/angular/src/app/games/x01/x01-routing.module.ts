import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { X01Component } from './x01.component';
import { X01RootComponent } from './x01-root.component';

const routes: Routes = [
  {
    path: "",
    component: X01RootComponent,
    //canActivate: [AuthorizationGuard],
    children: [
      {
        path: ':id',
        component: X01Component
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class X01RoutingModule { }