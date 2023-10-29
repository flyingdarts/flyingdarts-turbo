import { AfterViewInit, Component, ElementRef, ViewChild, OnInit, Input } from '@angular/core';
import { Carousel } from 'bootstrap';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.scss']
})
export class CarouselComponent implements AfterViewInit, OnInit {
  @Input() carouselItems: CarouselModel[] = []
  private carousel!: Carousel;

  constructor() {

  }
  ngOnInit(): void {
    var carouselElement = document.getElementById('registrationCarousel') as HTMLElement;
    this.carousel = new Carousel(carouselElement)
    if (this.carousel) {
      console.log(this.carousel.next());
    }
  }
  ngAfterViewInit(): void {
    // Set the interval for executing the code (e.g., every 5 seconds)
    const intervalSeconds = 5; // Set the interval in seconds
    const intervalMilliseconds = intervalSeconds * 1000; // Convert to milliseconds

    // Execute the code repeatedly at the specified interval
    setInterval(() => {
      if (this.carousel) {
        this.carousel.next();
      }
    }, intervalMilliseconds);
  }
}

export interface CarouselModel {
  src: string;
  title: string;
  description: string;
}