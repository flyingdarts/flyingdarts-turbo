import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { WebSocketService } from '../../infrastructure/websocket/websocket.service';
import { WebSocketActions } from '../../infrastructure/websocket/websocket.actions.enum';
import { MessageRequest } from '../../infrastructure/websocket/websocket.request.model';
import { WebSocketStatus } from '../../infrastructure/websocket/websocket.status.enum';
import { X01ApiService } from '../../services/x01-api.service';
import { AppStore } from 'src/app/app.store';
import { UserProfileStateService } from '../../services/user-profile-state.service';
import { Observable } from 'rxjs';
import { AppState } from 'src/app/app.state';
import { PreferedX01SettingsService } from 'src/app/services/prefered-x01-settings.service';
import { AnimationOptions } from 'ngx-lottie';
import { CreateX01GameCommand } from 'src/app/requests/CreateX01GameCommand';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { isNullOrUndefined } from 'src/app/app.component';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss'],
  animations: [
    trigger('leftToRight', [
      state('in', style({
        transform: 'translateX(0)',
        opacity: 1
      })),
      state('out', style({
        transform: 'translateX(100%)',
        opacity: 0
      })),
      transition('in => out', animate('300ms ease-out')),
      transition('out => in', animate('300ms ease-in'))
    ]),
    trigger('rightToLeft', [
      state('in', style({
        transform: 'translateX(0)',
        opacity: 1
      })),
      state('out', style({
        transform: 'translateX(100%)',
        opacity: 0
      })),
      transition('in => out', animate('300ms ease-in')),
      transition('out => in', animate('300ms ease-out'))
    ])
  ]
})
export class LobbyComponent implements OnInit {
  public loading!: boolean;
  public vm$: Observable<AppState>;
  public preferedX01Sets: number = 1;
  public preferedX01Legs: number = 3;

  public messages: MessageRequest[] = [];
  public webSocketStatus: WebSocketStatus = WebSocketStatus.Unknown
  public clientId: string = ""
  public playerId: string = "";
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/play.json'
  };
  public shouldHideLoader: boolean = true;
  public messageForm = new FormGroup({
    message: new FormControl(''),
  });
  constructor(
    private userProfileService: UserProfileStateService,
    private x01ApiService: X01ApiService,
    private router: Router,
    private webSocketService: WebSocketService,
    private store: AppStore,
    private settingsService: PreferedX01SettingsService,
    private activatedRoute: ActivatedRoute
  ) {
    this.store.select(x => x.loading).subscribe(x=> this.loading = x);
    this.vm$ = this.store.select(
      (state) => state
    );
  }

  ngOnInit(): void {
    this.loading = true;
    this.loadingTitle = this.getRandomTitle();
    this.loadingSubtitle = this.getRandomSubtitle();
    
    if (!isNullOrUndefined(this.settingsService.preferedX01Legs)) {
      this.settingsService.preferedX01Legs = 3;
      this.settingsService.preferedX01Sets = 1;
    }


    this.vm$.subscribe((x) => {
      this.preferedX01Sets = x.preferedSettings.x01Sets,
      this.preferedX01Legs = x.preferedSettings.x01Legs
    });

    this.webSocketService.getMessages().subscribe(x => {
      switch (x.action) {
        case WebSocketActions.Connect:
          this.webSocketStatus = WebSocketStatus.Connected
          break;
        case WebSocketActions.Default:
          this.messages.push(x.message! as MessageRequest)
          break;
        case WebSocketActions.Disconnect:
          this.webSocketStatus = WebSocketStatus.Disconnected
          break;
        case WebSocketActions.X01Create:
          this.shouldHideLoader = !this.shouldHideLoader;
          this.router.navigate(['/', 'x01', (x.message as CreateX01GameCommand).GameId!]);
          break;
        case WebSocketActions.X01JoinQueue:
          this.shouldHideLoader = !this.shouldHideLoader;
          this.router.navigate(['/', 'queue', 'x01',]);
          break;
      }
    })

    this.activatedRoute.data.subscribe(({ userProfile }) => {
      if (!isNullOrUndefined(userProfile)) {
        this.userProfileService.currentUserProfileDetails = userProfile;
        this.store.setProfile(userProfile);
        this.loading = false;
      }
    });
  }
  
  public joinX01Queue() {
    const userId = this.userProfileService.currentUserProfileDetails.UserId!
    this.shouldHideLoader = !this.shouldHideLoader;
    this.x01ApiService.joinQueue(userId);
  }

  public createX01Game() {
    const userId = this.userProfileService.currentUserProfileDetails.UserId!
    this.shouldHideLoader = !this.shouldHideLoader;
    var sets = this.settingsService.preferedX01Sets;
    var legs = this.settingsService.preferedX01Legs;
    this.x01ApiService.createGame(userId, sets, legs)
  }

  public shouldShowFriendSettings: boolean = false;

  public openFriendSettings() {
    this.shouldShowFriendSettings = !this.shouldShowFriendSettings;
  }

  public shouldShowFriendFaq: boolean = false;

  public openFriendFaq() {
    this.shouldShowFriendFaq = !this.shouldShowFriendFaq;
  }

  public shouldShowQueueFaq: boolean = false;

  public openQueueFaq() {
    this.shouldShowQueueFaq = !this.shouldShowQueueFaq;
  }

  public sendMessage() {
    this.webSocketService.postMessage(JSON.stringify({
      action: "message",
      message: {
        date: new Date(),
        message: this.messageForm.value.message,
        owner: this.clientId,
      }
    }))
  }
  public loadingTitle: string = 'Hang on!';
  public loadingSubtitle: string = 'Baking cookies...'

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
  public async increaseX01Legs() {
    if (this.preferedX01Legs < 13) {
      this.store.increasePreferedX01Legs();
    }
  }

  public async decreaseX01Legs() {
    if (this.preferedX01Legs >= 3) {
      this.store.decreasePreferedX01Legs();
    }
  }

  public async increaseX01Sets() {
    if (this.preferedX01Sets < 13) {
      this.store.increasePreferedX01Sets();
    }
  }

  public async decreaseX01Sets() {
    if (this.preferedX01Sets >= 3) {
      this.store.decreasePreferedX01Sets();
    }
  }

  public async saveX01Settings() {
    this.settingsService.preferedX01Sets = this.preferedX01Sets;
    this.settingsService.preferedX01Legs = this.preferedX01Legs;
    this.shouldShowFriendSettings = false;
  }
}
