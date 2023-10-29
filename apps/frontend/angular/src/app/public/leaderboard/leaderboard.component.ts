import { Component, OnDestroy, OnInit } from '@angular/core';
import { AnimationOptions } from 'ngx-lottie';
import { Subscription } from 'rxjs';
import { AmplifyAuthService } from './../../services/amplify-auth.service';

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit, OnDestroy {
  public isAuthenticated: boolean = false; // Initial value is false
  private isAuthenticatedSubscription?: Subscription = undefined;

  constructor(public authService: AmplifyAuthService) { }
  ngOnDestroy(): void {
    this.isAuthenticatedSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    this.isAuthenticatedSubscription = this.authService.isAuthenticated$.subscribe(
      (isAuthenticated: boolean) => {
        this.isAuthenticated = isAuthenticated;
      }
    );
  }
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_header.json',
    loop: false
  };
}
