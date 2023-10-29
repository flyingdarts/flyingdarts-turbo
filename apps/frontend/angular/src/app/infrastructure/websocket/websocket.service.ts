import { Injectable } from '@angular/core';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { environment } from './../../../environments/environment';
import { WebSocketActions } from './websocket.actions.enum';
import { WebSocketMessage } from './websocket.message.model';
import { WebSocketRequest } from './websocket.request.model';

@Injectable({ providedIn: 'root' })
export class WebSocketService<T = WebSocketRequest> {
  private socket!: WebSocket;
  private connectedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public connected$: Observable<boolean> = this.connectedSubject.asObservable();
  private messages = new Subject<WebSocketMessage<T>>();

  constructor() {
    this.initialize();
  }

  private initialize(): void {
    this.socket = new WebSocket(environment.webSocketUrl);
    this.connect();
  }

  private connect(): void {
    this.socket.onopen = (event) => {
      console.log(event);
      this.connectedSubject.next(true);
      this.messages.next({ action: WebSocketActions.Connect, message: event as any });
    };

    this.socket.onclose = (event) => {
      console.log(event);
      this.connectedSubject.next(false);
      this.messages.next({ action: WebSocketActions.Disconnect, message: event as any });
      console.log('disconnected from the websocket server');
      setTimeout(() => {
        console.log('attempting to re-establish connection with the websocket server');
        this.initialize();
      }, 1000);
    };

    this.socket.onerror = (event) => {
      console.log(event);
      this.messages.next({ action: WebSocketActions.Default, message: event as any });
    };

    this.socket.onmessage = (event) => {
      console.log(event);
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
