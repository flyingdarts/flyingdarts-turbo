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
  public loading$: Observable<boolean> = this.appStore.select(x => x.loading);
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'
  public showNoData = false;
  constructor(private plot: PlotlyService, private appStore: AppStore, private apiService: StatsApiService, private stateService: StatsStateService) { }

  addHoursToDate(date: Date, hoursToAdd: number): Date {
    const result = new Date(date);
    result.setHours(result.getHours() + hoursToAdd);
    return result;
  }

  ngOnInit(): void {
    var now = new Date();
    var nextDate = this.addHoursToDate(this.stateService.lastUpdated, 1);
    if (nextDate <= now) {
      this.stateService.darts = [];
      this.apiService.getStats().subscribe((statsArray: Array<Array<number>>) => {
        if (!isNullOrUndefined(statsArray)) {
          this.stateService.darts = statsArray;
          this.setPlot();
        }
      })
    } else {
      if (this.stateService.darts.length == 0) {
        this.showNoData = true;
      } else {
        this.setPlot();
      }
    }


    this.loadingTitle = this.getRandomTitle();
    this.loadingSubtitle = this.getRandomSubtitle();
  }
  refreshStats() {
    this.apiService.getStats().subscribe((statsArray: Array<Array<number>>) => {
      if (!isNullOrUndefined(statsArray)) {
        this.stateService.darts = statsArray;
        this.setPlot();
      }
    })
  }
  setPlot() {
    const days = this.stateService.darts.map((x) => x[0])
    // const days = Array.from({ length: 30 }, (_, i) => i + 1);
    const now = new Date();
    const startDate = new Date(`${now.getFullYear()}-${now.getMonth()}-01`); // Starting from February 1, 2024
    // const averages = [53, 50, 51, 54, 47, 53, 49, 46, 54, 52, 48, 54, 46, 49, 54, 48, 51, 55, 47, 50, 55, 47, 51, 55, 49, 48, 54, 48]; // Use your provided values
    const averages = this.stateService.darts.map((x) => x[1]);

    const dataModel = averages.map((average, index) => {
      const date = new Date(startDate);
      date.setDate(startDate.getDate() + index);
      return { average, date };
    });
    this.plot.plotLine("Last 30 day average", "plot", dataModel);
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
