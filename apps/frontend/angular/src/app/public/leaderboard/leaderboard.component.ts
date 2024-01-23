import { Component, OnDestroy, OnInit } from '@angular/core';
import { LoginClient } from '@authress/login';
import { AnimationOptions } from 'ngx-lottie';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit, OnDestroy {

  constructor() { }
  ngOnDestroy(): void {
  }

  ngOnInit(): void {

  }
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_header.json',
    loop: false
  };
}
