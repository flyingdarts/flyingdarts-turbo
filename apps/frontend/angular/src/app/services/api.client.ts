import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable, firstValueFrom } from "rxjs";
import { IApiClient } from "./IApiClient";
import { Injectable } from "@angular/core";
import { GetUserProfileQuery } from "../requests/GetUserProfileCommand";
import { UserProfileDetails } from "../shared/models/user-profile-details.model";
import { UpdateUserProfileCommand } from "../requests/UpdateUserProfileCommand";
import { CreateUserProfileCommand } from "../requests/CreateUserProfileCommand";

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


export class UsersService {
    private baseUrl: string;

    constructor(private httpClient: HttpClient) {
        this.baseUrl = '';
    }

    getUser(command: GetUserProfileQuery): Observable<UserProfileDetails> {
        return this.httpClient.get<UserProfileDetails>(`${this.baseUrl}/users/profile`, { params: { "CognitoUserName": command.CognitoUserName } });
    }

    updateUser(command: UpdateUserProfileCommand): Observable<UserProfileDetails> {
        return this.httpClient.put<UserProfileDetails>(`${this.baseUrl}/users/profile`, command);
    }

    createUser(command: CreateUserProfileCommand): Observable<UserProfileDetails> {
        return this.httpClient.post<UserProfileDetails>(`${this.baseUrl}/users/profile`, command);
    }
}