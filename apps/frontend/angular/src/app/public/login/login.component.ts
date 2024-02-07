import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { isNullOrUndefined } from 'src/app/app.component';
import { AppStore } from 'src/app/app.store';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { AuthressService } from 'src/app/services/authress_service';
import { UserProfileApiService } from 'src/app/services/user-profile-api.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {

  public loading$: Observable<boolean> = this.store.select(x => x.loading);
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'
  public canLogin: boolean = false;
  public accepted: boolean = false;
  constructor(
    private store: AppStore,
    private router: Router,
    private authressService: AuthressService,
    private stateService: UserProfileStateService,
    private activatedRoute: ActivatedRoute
    ) {

  }
  accept() {
    this.accepted = !this.accepted;
    this.canLogin = this.accepted;
  }


  async login() {
    await this.authressService.authenticate();
  }

  async ngOnInit() {
    this.store.setLoading(false)

    this.loadingTitle = this.getRandomTitle();
    this.loadingSubtitle = this.getRandomSubtitle();
    
    this.activatedRoute.data.subscribe(({ userProfileDetails }) => {
      if (!isNullOrUndefined(userProfileDetails)) {
        this.stateService.currentUserProfileDetails = userProfileDetails;
      }
    });

    try {
      var token = await this.authressService.getToken();
      var userId = this.authressService.getUserId();
      if (token.indexOf("user=") > -1) {
        token = token.substring(5)
      }
      this.stateService.idToken = token;
      if (!isNullOrUndefined(userId)) {
        if (isNullOrUndefined(this.stateService.currentUserProfileDetails)) {
          this.router.navigate(['/', 'onboarding', { outlets: { 'onboarding-outlet': ['profile'] } }])
        } else {
          this.router.navigate(['/', 'lobby'])
        }
      }
    } catch(error) {
      console.log(error);
      this.store.setLoading(false);
    }
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