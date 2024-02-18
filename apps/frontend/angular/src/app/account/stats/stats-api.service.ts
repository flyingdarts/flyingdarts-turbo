import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StatsApiService {
  private baseHref = "";
  constructor(private httpClient: HttpClient) {
    this.baseHref = environment.statsApi;
  }
  public getStats(startDate: Date, endDate: Date) {
    try {
      var parsedStartDate = startDate.toLocaleDateString();
      var parsedEndDate = endDate.toLocaleDateString();
      return this.httpClient.get<Array<Stats>>(`${this.baseHref}/stats`, { params: { startDate: parsedStartDate, endDate: parsedEndDate }})
    } catch (error) {
      console.error(error);
      return of(Array<Stats>());
    }
  }
}



