import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AppStore } from 'src/app/app.store';
import { PlotlyService } from 'src/app/services/plotly.service';
import { StatsApiService } from './stats-api.service';
import { StatsStateService } from './stats-state.service';
import { isNullOrUndefined } from 'src/app/app.component';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit {
  public loading!: boolean;
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'
  public showPlot = false;
  constructor(private plot: PlotlyService, private appStore: AppStore, private apiService: StatsApiService, private stateService: StatsStateService) {  }


  ngOnInit(): void {
    this.loading = true;
    this.loadingTitle = this.getRandomTitle();
    this.loadingSubtitle = this.getRandomSubtitle();
    console.log('[StatsComponent] Initiated')
    if (this.stateService.darts.length > 0) {
      console.log('[StatsComponent] State has darts', this.stateService.darts)
      this.showPlot = true;
      this.setPlot();
      this.loading = false;
    } else {
      console.log('[StatsComponent] Fetching darts', this.stateService.darts)
      this.refreshStats();
      this.loading = false;
    }
  }

  _loadStats(startDate: Date, endDate: Date) { 
    this.apiService.getStats(startDate, endDate).subscribe((statsArray: Array<Stats>) => {
      if (!isNullOrUndefined(statsArray)) {
        this.stateService.darts = statsArray;
        this.showPlot = true;
        this.setPlot();
      }
    })
  }
  refreshStats() {
    const now = new Date();
    const endDate  = this.addHoursToDate(now, 24 * 7);
    this._loadStats(now, endDate);
  }
  setPlot() {
    this.plot.plotLine("Last 30 day average", "plot", this.stateService.darts);
    this.appStore.setLoading(false)
  }
  getRandomTitle(): string {
    var random = Math.floor(Math.random() * this.loadingTexts.length);
    return this.loadingTexts[random].title;
  }

  getRandomSubtitle() {
    var random = Math.floor(Math.random() * this.loadingTexts.length);
    return this.loadingTexts[random].subtitle;
  }

  addHoursToDate(date: Date, hoursToAdd: number): Date {
    const result = new Date(date);
    result.setHours(result.getHours() + hoursToAdd);
    return result;
  }
  loadingTexts = [
    {
      "title": "Hang on!",
      "subtitle": "Sharpening the darts..."
    },
    {
      "title": "Almost there!",
      "subtitle": "Polishing the dartboard..."
    },
    {
      "title": "Hold tight!",
      "subtitle": "Aligning the scorecard..."
    },
    {
      "title": "Just a moment!",
      "subtitle": "Calibrating the oche..."
    },
    {
      "title": "One second!",
      "subtitle": "Gathering the flights..."
    },
    {
      "title": "Patience is key!",
      "subtitle": "Checking the bullseye..."
    },
    {
      "title": "Ready soon!",
      "subtitle": "Lacing up the dart shoes..."
    },
    {
      "title": "Almost ready!",
      "subtitle": "Preparing the chalk..."
    },
    {
      "title": "Hang in there!",
      "subtitle": "Finalizing the throw line..."
    },
    {
      "title": "Preparing!",
      "subtitle": "Setting up the lighting..."
    },
    {
      "title": "Getting close!",
      "subtitle": "Organizing the darts league..."
    },
    {
      "title": "Finishing touches!",
      "subtitle": "Testing the scoreboard..."
    },
    {
      "title": "Bear with us!",
      "subtitle": "Selecting the music playlist..."
    },
    {
      "title": "Stay tuned!",
      "subtitle": "Adjusting the dart grip..."
    },
    {
      "title": "Wrapping up!",
      "subtitle": "Checking the dart weights..."
    },
    {
      "title": "Countdown!",
      "subtitle": "Planning the victory celebration..."
    },
    {
      "title": "Finalizing!",
      "subtitle": "Reviewing the rules..."
    },
    {
      "title": "Almost done!",
      "subtitle": "Warming up the players..."
    },
    {
      "title": "Preparing the stage!",
      "subtitle": "Securing the dart cases..."
    },
    {
      "title": "Ready to throw!",
      "subtitle": "Ensuring a fair play..."
    }
  ];
}
