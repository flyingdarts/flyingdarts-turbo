import { BehaviorSubject, Observable, Subject } from "rxjs";
import { environment } from "./../../../environments/environment";
import { WebSocketActions } from "./websocket.actions.enum";
import { WebSocketMessage } from "./websocket.message.model";
import { WebSocketRequest } from "./websocket.request.model";

export class WebSocketService {
  private socket!: WebSocket;
  private isConnectedSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);

  public isConnected$: Observable<boolean> =
    this.isConnectedSubject.asObservable();

  private messagesSubject = new Subject<WebSocketMessage<WebSocketRequest>>();
  public messages$: Observable<WebSocketMessage<WebSocketRequest>> =
    this.messagesSubject.asObservable();

  // Event listeners for specific actions
  private actionListeners: Map<
    string,
    ((message: WebSocketMessage<WebSocketRequest>) => void)[]
  > = new Map();

  constructor(getTokenCallback: () => Promise<string>) {
    getTokenCallback().then((token) => {
      const idToken = localStorage.getItem("AuthenticationCredentialsStorage")!;
      const idTokenParsed = JSON.parse(idToken);
      const idTokenValue = idTokenParsed.idToken;

      var url =
        environment.webSocketUrl + `?token=${encodeURIComponent(token)}`;
      if (idTokenValue) {
        url = url + `&idToken=${encodeURIComponent(idTokenValue)}`;
      }
      this.socket = new WebSocket(url);
      this.connect();
    });
  }

  private connect(): void {
    this.socket.onopen = (event) => {
      this.isConnectedSubject.next(true);
      const message: WebSocketMessage<WebSocketRequest> = {
        action: WebSocketActions.Connect,
        message: event as any,
      };
      this.messagesSubject.next(message);
      this.triggerActionListeners(message);
    };

    this.socket.onclose = (event) => {
      this.isConnectedSubject.next(false);
      const message: WebSocketMessage<WebSocketRequest> = {
        action: WebSocketActions.Disconnect,
        message: event as any,
      };
      this.messagesSubject.next(message);
      this.triggerActionListeners(message);
      setTimeout(() => {
        // this.initialize();  // TODO: Should handle reconnection
      }, 1000);
    };

    this.socket.onerror = (event) => {
      const message: WebSocketMessage<WebSocketRequest> = {
        action: WebSocketActions.Default,
        message: event as any,
      };
      this.messagesSubject.next(message);
      this.triggerActionListeners(message);
    };

    this.socket.onmessage = (event) => {
      let messageData = JSON.parse(event.data);
      const message: WebSocketMessage<WebSocketRequest> = {
        action: messageData.action,
        message: messageData.message,
        metadata: messageData.metadata,
      };
      this.messagesSubject.next(message);
      this.triggerActionListeners(message);
    };
  }

  private triggerActionListeners(
    message: WebSocketMessage<WebSocketRequest>
  ): void {
    const listeners = this.actionListeners.get(message.action);
    if (listeners) {
      listeners.forEach((callback) => callback(message));
    }
  }

  /**
   * Register a callback for a specific WebSocket action
   * @param action The action name to listen for
   * @param callback The callback function to execute when the action is received
   */
  public on(
    action: string,
    callback: (message: WebSocketMessage<WebSocketRequest>) => void
  ): void {
    if (!this.actionListeners.has(action)) {
      this.actionListeners.set(action, []);
    }
    this.actionListeners.get(action)!.push(callback);
  }

  /**
   * Unregister a callback for a specific WebSocket action
   * @param action The action name to stop listening for
   * @param callback The specific callback function to remove (optional - if not provided, all callbacks for the action are removed)
   */
  public off(
    action: string,
    callback?: (message: WebSocketMessage<WebSocketRequest>) => void
  ): void {
    if (!this.actionListeners.has(action)) {
      return;
    }

    if (!callback) {
      // Remove all callbacks for this action
      this.actionListeners.delete(action);
    } else {
      // Remove specific callback
      const listeners = this.actionListeners.get(action)!;
      const index = listeners.indexOf(callback);
      if (index > -1) {
        listeners.splice(index, 1);
      }
      // Clean up empty arrays
      if (listeners.length === 0) {
        this.actionListeners.delete(action);
      }
    }
  }

  public postMessage(payload: string): void {
    if (this.isConnectedSubject.value) {
      this.socket.send(payload);
    }
  }
}
