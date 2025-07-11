import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from './../../../environments/environment';
import { WebSocketActions } from './websocket.actions.enum';
import { WebSocketMessage } from './websocket.message.model';
import { WebSocketRequest } from './websocket.request.model';

export class WebSocketService {
  private socket!: WebSocket;
  private isConnectedSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);

  public isConnected$: Observable<boolean> =
    this.isConnectedSubject.asObservable();

  private messagesSubject = new Subject<WebSocketMessage<WebSocketRequest>>();
  public messages$: Observable<WebSocketMessage<WebSocketRequest>> =
    this.messagesSubject.asObservable();

  constructor(getTokenCallback: () => Promise<string>) {
    getTokenCallback().then((token) => {
      this.socket = new WebSocket(
        environment.webSocketUrl + `?token=${encodeURIComponent(token)}`
      );
      this.connect();
    });
  }

  private connect(): void {
    this.socket.onopen = (event) => {
      this.isConnectedSubject.next(true);
      this.messagesSubject.next({
        action: WebSocketActions.Connect,
        message: event as any,
      });
    };

    this.socket.onclose = (event) => {
      this.isConnectedSubject.next(false);
      this.messagesSubject.next({
        action: WebSocketActions.Disconnect,
        message: event as any,
      });
      setTimeout(() => {
        // this.initialize();  // TODO: Should handle reconnection
      }, 1000);
    };

    this.socket.onerror = (event) => {
      this.messagesSubject.next({
        action: WebSocketActions.Default,
        message: event as any,
      });
    };

    this.socket.onmessage = (event) => {
      let message = JSON.parse(event.data);
      this.messagesSubject.next({
        action: message.action,
        message: message.message,
        metadata: message.metadata,
      });
    };
  }

  public postMessage(payload: string): void {
    if (this.isConnectedSubject.value) {
      this.socket.send(payload);
    }
  }
}
