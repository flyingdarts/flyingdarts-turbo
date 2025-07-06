import { Injectable } from "@angular/core";
import { LoginClient, Settings } from "@authress/login";
import { environment } from "src/environments/environment";

@Injectable({providedIn: 'root'})
export class AuthressService {
    private loginClient: LoginClient;

    constructor() {
        var loginClientSettings: Settings = {
            applicationId: environment.authApplicationId,
            authressApiUrl:environment.authLoginUrl,
          }
          this.loginClient = new LoginClient(loginClientSettings);
      
    }

    public async authenticate(): Promise<boolean> {
        const response = await this.loginClient.authenticate({redirectUrl: window.location.href});
        return response !== null;
    }
    
    public async getToken(): Promise<string | null> {
        try { 
            return await this.loginClient.ensureToken();
        } catch(err) {
            return null;
        }
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