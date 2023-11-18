import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StatsRoutingModule } from './stats-routing.module';
import { X01StatsComponent } from './x01-stats/x01-stats.component';
import { StatsRootComponent } from './stats-root.component';
import { PlotlyService } from '../services/plotly.service';


@NgModule({
  declarations: [
    StatsRootComponent,
    X01StatsComponent
  ],
  imports: [
    CommonModule,
    StatsRoutingModule
  ],
  providers: [
    PlotlyService
  ]
})
export class StatsModule { }
