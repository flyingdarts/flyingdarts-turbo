import { Component, HostListener, OnInit } from '@angular/core';
import { AnimationItem } from 'lottie-web';
import { AnimationOptions } from 'ngx-lottie';
import packageJson from "./../../package.json";
import * as AuthActions from './auth.actions';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppStore } from './app.store';
import { AuthressService } from './services/authress_service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public currentYear: number = new Date().getFullYear();
  public currentVersion: string = "";
  public loading$: Observable<boolean> = this.appStore.select(x => x.loading);
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'
  
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: false
  };

  constructor(
    private store: Store,
    private appStore: AppStore,
    private router: Router,
    private authressService: AuthressService) {
    this.currentVersion = packageJson.version;
  }
  
  async ngOnInit() {
    this.appStore.setLoading(false);

    this.store.dispatch(AuthActions.listenForAuthEvents());

    this.loadingTitle = this.getRandomTitle();
    this.loadingSubtitle = this.getRandomSubtitle();
    if (await this.authressService.isUserLoggedIn()) {
      this.router.navigate(['/', 'lobby'])
    } else {
      this.router.navigate(['/', 'login'])
    }
  }

  onAnimate(animationItem: AnimationItem): void {
  }

  title = 'Flyingdarts';

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
export function isNullOrUndefined(value: any): boolean {
  return value == null || value == undefined
}



