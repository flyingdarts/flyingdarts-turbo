import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { StatsDart } from './stats-state.service';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';

@Injectable({ providedIn: 'root' })
export class StatsApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient,
     private stateService: UserProfileStateService) {
    this.baseHref = environment.statsApi;
  }
  public getStats() {
    var headers = { Authorization: this.stateService.idToken };

    return this.httpClient.get<Array<StatsDart>>('stats', { headers: headers })
  }
}



