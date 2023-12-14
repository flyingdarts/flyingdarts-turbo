import { Component, Input, OnInit } from '@angular/core';
import { timeInterval } from 'rxjs';
import { LoadingService } from './../../services/loading.service';
import { AnimationOptions } from 'ngx-lottie';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss']
})
export class LoadingComponent implements OnInit {
  @Input() title!: string;
  @Input() subtitle!: string;
  @Input() lottieOptions: AnimationOptions = {
    path: '/assets/animations/flyingdarts_header.json'
  };
  constructor(public loader: LoadingService) {
  }
  ngOnInit(): void {
    timeInterval()
  }
}