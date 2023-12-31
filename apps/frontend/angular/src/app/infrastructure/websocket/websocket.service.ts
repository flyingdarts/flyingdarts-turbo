import { Injectable } from '@angular/core';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { environment } from './../../../environments/environment';
import { WebSocketActions } from './websocket.actions.enum';
import { WebSocketMessage } from './websocket.message.model';
import { WebSocketRequest } from './websocket.request.model';
import { AmplifyAuthService } from 'src/app/services/amplify-auth.service';

@Injectable({ providedIn: 'root' })
export class WebSocketService<T = WebSocketRequest> {
  private socket!: WebSocket;
  private connectedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public connected$: Observable<boolean> = this.connectedSubject.asObservable();
  private messages = new Subject<WebSocketMessage<T>>();

  constructor(private amplifyAuthService: AmplifyAuthService) {
    this.initialize();
  }

  private initialize(): void {
    let wsUrl = environment.webSocketUrl;
    this.amplifyAuthService.getAccessToken()
      .then((result) => {
        // The code here will be executed after someAsyncFunction resolves
        wsUrl += `?token=${encodeURIComponent(result)}`;
        this.socket = new WebSocket(wsUrl);
        this.connect();
      })
      .catch((error) => {
        console.log('cant get auth token');
        console.error(error);
      });
  }

  private connect(): void {
    this.socket.onopen = (event) => {
      this.connectedSubject.next(true);
      this.messages.next({ action: WebSocketActions.Connect, message: event as any });
    };

    this.socket.onclose = (event) => {
      this.connectedSubject.next(false);
      this.messages.next({ action: WebSocketActions.Disconnect, message: event as any });
      setTimeout(() => {
        this.initialize();
      }, 1000);
    };

    this.socket.onerror = (event) => {
      this.messages.next({ action: WebSocketActions.Default, message: event as any });
    };

    this.socket.onmessage = (event) => {
      let message = JSON.parse(event.data);
      this.messages.next({ action: message.action, message: message.message, metadata: message.metadata });
    };
  }

  public postMessage(payload: string): void {
    if (this.connectedSubject.value) {
      this.socket.send(payload);
    }
  }

  public getMessages(): Observable<WebSocketMessage<T>> {
    return this.messages.asObservable();
  }
}
