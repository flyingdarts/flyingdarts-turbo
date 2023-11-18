import { Observable } from "rxjs";

export interface IApiClient {
    get<TRequest>(endpoint: string): Observable<TRequest>;
    post<TRequest>(endpoint: string, request: TRequest): void;
    put<TRequest>(endpoint: string, request: TRequest): void;
    delete<TRequest>(endpoint: string, request: TRequest): void;
}
