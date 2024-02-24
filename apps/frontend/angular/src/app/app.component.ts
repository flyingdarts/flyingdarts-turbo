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
export class AppComponent {
  public currentYear: number = new Date().getFullYear();
  public currentVersion: string = "";

  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: false
  };

  constructor(private appStore: AppStore) {
    this.currentVersion = packageJson.version;
  }

  onAnimate(animationItem: AnimationItem): void {
  }

  title = 'flyingdarts';
}
export function isNullOrUndefined(value: any): boolean {
  return value == null || value == undefined
}



