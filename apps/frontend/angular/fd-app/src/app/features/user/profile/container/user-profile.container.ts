import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppStateSelectors } from 'src/app/state/app';

@Component({
  selector: 'app-user-profile-ui',
  imports: [CommonModule],
  templateUrl: './user-profile.container.html',
  standalone: true,
})
export class UserProfileComponent {
  public userName$!: Observable<string>;

  constructor(private store: Store) {
    this.userName$ = this.store.select(AppStateSelectors.selectUser).pipe(map(data => data?.UserName ?? ''));
  }

  onSubmit(userName: string): void {
    // submit to server
  }
}
