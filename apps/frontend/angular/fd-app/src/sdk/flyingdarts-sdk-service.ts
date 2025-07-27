import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StateHooksService } from 'src/app/services/state-hooks.service';
import { FlyingdartsSdk } from 'src/sdk/flyingdarts-sdk';
import { FlyingdartsSdkFactory } from './flyingdarts-sdk-factory';

@Injectable({ providedIn: 'root' })
export class FlyingdartsSdkService {
  private sdkInstance: FlyingdartsSdk | undefined;

  constructor(
    private readonly stateService: StateHooksService,
    private readonly httpClient: HttpClient
  ) {}

  public get instance(): FlyingdartsSdk | undefined {
    return this.sdkInstance;
  }

  // lets init the sdk
  initSdk(): FlyingdartsSdk {
    if (!this.sdkInstance) {
      this.sdkInstance = new FlyingdartsSdkFactory(
        this.stateService,
        this.httpClient
      ).create();
    }
    return this.sdkInstance;
  }
}
