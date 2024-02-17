import { Component, Input, OnInit } from '@angular/core';
import { timeInterval } from 'rxjs';
import { LoadingService } from './../../services/loading.service';
import {
  AnimationOptions,
  LottieComponent,
  provideLottieOptions,
} from 'ngx-lottie';
import { lottiePlayerFactory } from '../lottiePlayerFactory';
@Component({
  selector: 'app-loading',
  standalone: true,
  templateUrl: './loading.component.html',
  imports: [LottieComponent],
  styleUrls: ['./loading.component.scss'],
  providers: [provideLottieOptions(lottiePlayerFactory())]
})
export class LoadingComponent implements OnInit {
  @Input() title!: string;
  @Input() subtitle!: string;
  @Input() lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_header.json'
  };
  public options: AnimationOptions;
  constructor(public loader: LoadingService) {
    this.options = this.lottieOptions;
  }
  ngOnInit(): void {
    timeInterval()
  }
}