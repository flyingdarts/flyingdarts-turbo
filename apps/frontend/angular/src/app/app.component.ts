import { Component, HostListener, OnInit } from '@angular/core';
import { AnimationItem } from 'lottie-web';
import { AnimationOptions } from 'ngx-lottie';
import packageJson from "./../../package.json";
import * as AuthActions from './auth.actions';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public currentYear: number = new Date().getFullYear();
  public currentVersion: string = "";

  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: false
  };

  constructor(
    private store: Store) {
    this.currentVersion = packageJson.version;
  }
  
  ngOnInit() {
    this.store.dispatch(AuthActions.listenForAuthEvents());
  }

  onAnimate(animationItem: AnimationItem): void {
    console.log(animationItem);
  }

  title = 'Flyingdarts';
}
export function isNullOrUndefined(value: any): boolean {
  return value == null || value == undefined
}



