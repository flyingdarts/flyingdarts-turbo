import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AnimationOptions } from 'ngx-lottie';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: true
  };

  public settingsForm: FormGroup = new FormGroup({});
  public isLoading: boolean = false;
  public loadingTitle: string = "loading";
  public loadingSubtitle: string = "is so slooow";

  constructor() { }

  ngOnInit(): void {
  }
  public updateSettings() {

  }
}