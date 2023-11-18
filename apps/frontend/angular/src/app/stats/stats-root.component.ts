import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-games-root',
  template: `
    <router-outlet></router-outlet>
  `,
  styles: [`
    /* Add your styles here */
  `]
})
export class StatsRootComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}