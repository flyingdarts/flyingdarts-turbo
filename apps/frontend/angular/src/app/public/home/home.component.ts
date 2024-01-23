import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AppStore } from 'src/app/app.store';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public loading$: Observable<boolean> = this.store.select(x => x.loading);

  constructor(private store: AppStore) { }

  ngOnInit(): void {
    
  }

}
