import { StateHooksService } from 'src/app/services/state-hooks.service';
import { WebSocketActions } from 'src/app/services/websocket/websocket.actions.enum';
import { WebSocketService } from 'src/app/services/websocket/websocket.service';

export class FlyingdartsRepository {
  constructor(public readonly webSocketService: WebSocketService, private readonly stateHooksService: StateHooksService) {
    this.webSocketService.messages$.subscribe(message => {
      if (message.action === WebSocketActions.X01Create) {
        const gameId = message.metadata?.Game.Id;
        const meetingId = message.metadata?.MeetingIdentifier;
        const meetingToken = message.metadata?.MeetingToken;
        if (meetingId) {
          sessionStorage.setItem('meetingId', meetingId);
        }
        if (meetingToken) {
          sessionStorage.setItem('meetingToken', meetingToken);
        }
        if (gameId) {
          this.stateHooksService.handleGame(gameId);
        }
      } else if (message.action === WebSocketActions.X01Join) {
        const gameId = message.metadata?.Game.Id;
        const meetingId = message.metadata?.MeetingIdentifier;
        const meetingToken = message.metadata?.MeetingToken;
        if (meetingId) {
          sessionStorage.setItem('meetingId', meetingId);
        }
        if (meetingToken) {
          sessionStorage.setItem('meetingToken', meetingToken);
        }
        if (gameId) {
          this.stateHooksService.handleGame(gameId);
        }
      }
    });
  }
}
