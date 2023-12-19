import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-queue-root',
  template: `
    <router-outlet></router-outlet>
  `,
  styles: [`
    /* Add your styles here */
  `]
})
export class QueueRootComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}