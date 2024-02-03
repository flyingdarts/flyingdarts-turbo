import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AppStore } from 'src/app/app.store';
import { PlotlyService } from 'src/app/services/plotly.service';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit {
  public loading$: Observable<boolean> = this.appStore.select(x => x.loading);
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'
  constructor(private plot:PlotlyService, private appStore: AppStore) { }
  ngOnInit(): void {
    const days = Array.from({ length: 30 }, (_, i) => i + 1);
    const startDate = new Date('2024-02-01'); // Starting from February 1, 2024
    const averages = [53, 50, 51, 54, 47, 53, 49, 46, 54, 52, 48, 54, 46, 49, 54, 48, 51, 55, 47, 50, 55, 47, 51, 55, 49, 48, 54, 48]; // Use your provided values
    
    const dataModel = averages.map((average, index) => {
      const date = new Date(startDate);
      date.setDate(startDate.getDate() + index);
      return { average, date };
    });
    this.plot.plotLine("Last 30 day average", "plot", dataModel);
    this.appStore.setLoading(false)
  
      this.loadingTitle = this.getRandomTitle();
      this.loadingSubtitle = this.getRandomSubtitle();
    }
    getRandomTitle(): string {
      var random= Math.floor(Math.random() * this.loadingTexts.length);
      return this.loadingTexts[random].title;
    }
  
    getRandomSubtitle() {
      var random= Math.floor(Math.random() * this.loadingTexts.length);
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
