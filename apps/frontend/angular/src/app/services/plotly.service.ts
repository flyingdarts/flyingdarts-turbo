import { Injectable } from '@angular/core';
import * as Plotly from 'plotly.js-dist-min';

@Injectable({
  providedIn: 'root'
})
export class PlotlyService {
  constructor() { }

  plotLine(title: string, plotDiv: string, data: Array<Stats>) {
    var first = data[0];
    console.log(first.Day!);
    const x = data.map(item => item.Day); // Format date as 'YYYY-MM-DD'
    const y = data.map(item => item.Average);
  
    let trace = {
      x: x,
      y: y,
      type: 'scatter',
      mode: 'lines+markers',
      marker: {
        color: '#4c56c0',
        size: 8
      },
      line: {
        color: '#f9617d',
        width: 2
      }
    };
  
    const startDateStr = data[0].Day;
    const endDateStr = data[data.length-1].Day;
  
    let layout = {
      title: title,
      paper_bgcolor: 'rgba(37, 45, 73, 0.7)',
      plot_bgcolor: 'rgba(37, 45, 73, 0.7)',
      font: {
        color: '#4c56c0',
      },
      xaxis: {
        range: [startDateStr, endDateStr], // Explicitly set the range of the x-axis
        tickvals: x, // Specify tick values to correspond with each date
        ticktext: x.map(date => date!.toString().substring(0, 10)), // Customize tick labels
        tickangle: -45,
      },
      // Other layout customizations...
    };
    console.log(plotDiv);
    console.log(trace);
    console.log(layout);
    Plotly.newPlot(plotDiv, [trace], layout);
  }
  
  
}
