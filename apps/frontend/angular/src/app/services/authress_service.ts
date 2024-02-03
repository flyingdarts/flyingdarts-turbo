import { Injectable } from "@angular/core";
import { LoginClient } from "@authress/login";
import { environment } from "src/environments/environment";

@Injectable({providedIn: 'root'})
export class AuthressService {
    private loginClient: LoginClient;

    constructor() {
        this.loginClient = new LoginClient({
            authressLoginHostUrl: environment.authLoginUrl, 
            applicationId: environment.authApplicationId,
          });
    }

    public authenticate(): Promise<boolean> {
        return this.loginClient.authenticate({redirectUrl: window.location.href})
    }
    
    public async getToken(): Promise<string> {
        return await this.loginClient.ensureToken();
    }

    public getUserId(): string {
        var userIdentity = this.loginClient.getUserIdentity();
        var userId: string = userIdentity["userId"] as string;
        return userId;
    }

    public signout(): Promise<void> {
        return this.loginClient.logout(window.location.href)
    }

    public isUserLoggedIn(): Promise<boolean> {
        return this.loginClient.userSessionExists();
    }
}