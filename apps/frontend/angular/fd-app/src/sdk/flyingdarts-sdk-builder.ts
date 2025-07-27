import { HttpClient } from "@angular/common/http";
import { AuthressService } from "src/app/services/authress.service";
import { StateHooksService } from "src/app/services/state-hooks.service";
import { WebSocketMessageQueueService } from "src/app/services/websocket/websocket-message-queue.service";
import { WebSocketService } from "src/app/services/websocket/websocket.service";
import { environment } from "src/environments/environment";
import { ApiConfig } from "./config/api.config";
import { FlyingdartsSdk } from "./flyingdarts-sdk";
import { FlyingdartsRepository } from "./flyingdarts.repository";
import { LoginClient } from "@mikepattyn/authress-angular";

export class FlyingdartsSdkBuilder {
  private apiConfig!: ApiConfig;
  private stateHooksService!: StateHooksService;
  private httpClient!: HttpClient;

  setupHttpClient(httpClient: HttpClient): FlyingdartsSdkBuilder {
    this.httpClient = httpClient;
    return this;
  }
  setupApiConfig(
    webSocketUrl: string,
    usersApiUrl: string
  ): FlyingdartsSdkBuilder {
    this.apiConfig = {
      webSocketUrl,
      usersApiUrl,
    };
    return this;
  }
  setupHooks(stateHooksService: StateHooksService): FlyingdartsSdkBuilder {
    this.stateHooksService = stateHooksService;
    return this;
  }

  build(): FlyingdartsSdk {
    if (!this.apiConfig) {
      throw new Error("Missing configuration");
    }

    // Initialize LoginClient with correct parameters
    const loginClient = new LoginClient({
      authressApiUrl: environment.authressLoginUrl,
      applicationId: environment.authressAppId,
    });

    // Pass the initialized LoginClient to AuthressService
    const authressService = new AuthressService(loginClient);

    // Bind getToken method to ensure correct context
    const getTokenBound = authressService.getToken.bind(authressService);

    // Use the bound method for WebSocketService
    const webSocketService = new WebSocketService(getTokenBound);
    const webSocketMessageQueueService = new WebSocketMessageQueueService(
      webSocketService
    );

    const flyingdartsRepository = new FlyingdartsRepository(
      webSocketService,
      this.stateHooksService
    );

    return new FlyingdartsSdk(
      webSocketService,
      webSocketMessageQueueService,
      flyingdartsRepository
    );
  }
}
