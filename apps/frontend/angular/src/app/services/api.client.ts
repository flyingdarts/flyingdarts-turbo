import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable, firstValueFrom } from "rxjs";
import { IApiClient } from "./IApiClient";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class ApiClient implements IApiClient {
    defaultHeaders: HttpHeaders;

    constructor(private http: HttpClient) {
        this.defaultHeaders = new HttpHeaders({
            "Accept": "application/json",
        });
    }

    get<TResponse>(endpoint: string, params?: { [key: string]: any; }): Observable<TResponse> {
        var headers = this.defaultHeaders;
        let queryParams = new HttpParams();
        if (params) {
            for (const key in params) {
                if (params.hasOwnProperty(key)) {
                    queryParams = queryParams.set(key, params[key]);
                }
            }
        }
        return this.http.get<TResponse>(endpoint, { headers: headers, params: params });
    }

    async post<TRequest, TResponse>(endpoint: string, request: TRequest) {
        var headers = this.defaultHeaders;
        return await firstValueFrom(this.http.post<TResponse>(endpoint, request, { headers: headers }));
    }

    async put<TRequest, TResponse>(endpoint: string, request: TRequest) {
        var headers = this.defaultHeaders;
        return await firstValueFrom(this.http.put<TResponse>(endpoint, request, { headers: headers }));
    }

    delete<TRequest>(endpoint: string, request: TRequest): void {
        throw new Error("Method not implemented.");
    }

}