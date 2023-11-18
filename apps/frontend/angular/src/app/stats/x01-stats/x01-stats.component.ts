import { Component, OnInit } from '@angular/core';
import { PlotlyService } from 'src/app/services/plotly.service';

@Component({
  selector: 'app-x01-stats',
  templateUrl: './x01-stats.component.html',
  styleUrls: ['./x01-stats.component.scss']
})
export class X01StatsComponent implements OnInit {

  constructor(private plot:PlotlyService) { }
  ngOnInit(): void {
    const days = Array.from({ length: 30 }, (_, i) => i + 1);
    const throws = [53, 50, 51, 54, 47, 53, 49, 46, 54, 52, 48, 54, 46, 49, 54, 48, 51, 55, 47, 50, 55, 47, 51, 55, 49, 48, 54, 48, 53, 47];
      this.plot.plotLine("Last 30 day average","plot",days, throws);
    }

}
