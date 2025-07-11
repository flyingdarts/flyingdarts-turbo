import { Injectable } from '@angular/core';
import DyteClient from '@dytesdk/web-core';

@Injectable({ providedIn: 'root' })
export class DyteClientFactory {
  async create(authToken: string): Promise<DyteClient> {
    return await DyteClient.init({
      authToken: authToken,
      defaults: {
        audio: true,
        video: true,
      },
    });
  }
}
