import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StateHooksService } from 'src/app/services/state-hooks.service';
import { environment } from 'src/environments/environment';
import { FlyingdartsSdk } from 'src/sdk/flyingdarts-sdk';
import { FlyingdartsSdkBuilder } from './flyingdarts-sdk-builder';

@Injectable({ providedIn: 'root' })
export class FlyingdartsSdkFactory {
  constructor(private readonly stateHooksService: StateHooksService, private readonly httpClient: HttpClient) {}

  create(): FlyingdartsSdk {
    return new FlyingdartsSdkBuilder()
      .setupHttpClient(this.httpClient)
      .setupHooks(this.stateHooksService)
      .setupApiConfig(environment.webSocketUrl, environment.usersApi)
      .build();
  }
}
