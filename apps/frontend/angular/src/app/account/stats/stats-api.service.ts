import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { StatsDart } from './stats-state.service';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StatsApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient,
     private stateService: UserProfileStateService) {
    this.baseHref = environment.statsApi;
  }
  public getStats() {
    try {
      var headers = { Authorization: this.stateService.idToken };

      return this.httpClient.get<Array<Array<number>>>('stats', { headers: headers })
    } catch (error) {
      console.error(error);
      return of(Array<Array<number>>());
    }
  }
}



