import { Injectable } from '@angular/core';
import * as Plotly from 'plotly.js-dist-min';

@Injectable({
  providedIn: 'root'
})
export class PlotlyService {
  constructor() { }

  plotLine(title: string, plotDiv: string, data: { average: number, date: Date }[]) {
    const x = data.map(item => item.date.toISOString().substring(0, 10)); // Format date as 'YYYY-MM-DD'
    const y = data.map(item => item.average);
  
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
  
    const startDateStr = data[0].date.toISOString().substring(0, 10);
    const endDate = new Date(data[data.length - 1].date);
    endDate.setDate(endDate.getDate() + 1); // Ensure the end date is included
    const endDateStr = endDate.toISOString().substring(0, 10);
  
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
        ticktext: x.map(date => new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })), // Customize tick labels
        tickangle: -45,
      },
      // Other layout customizations...
    };
  
    Plotly.newPlot(plotDiv, [trace], layout);
  }
  
  
}
