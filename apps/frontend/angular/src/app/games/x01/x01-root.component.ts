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
export class X01RootComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}