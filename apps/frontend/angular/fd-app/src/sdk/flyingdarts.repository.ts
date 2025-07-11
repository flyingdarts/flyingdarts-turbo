import { filter, firstValueFrom, tap } from 'rxjs';
import { MeetingsRepository } from 'src/app/repositories/meetings.repository';
import { CreateX01GameCommand } from 'src/app/requests/CreateX01GameCommand';
import { StateHooksService } from 'src/app/services/state-hooks.service';
import { WebSocketActions } from 'src/app/services/websocket/websocket.actions.enum';
import { WebSocketService } from 'src/app/services/websocket/websocket.service';

export class FlyingdartsRepository {
  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly meetingsRepository: MeetingsRepository,
    private readonly stateHooksService: StateHooksService
  ) {
    this.webSocketService.messages$
      .pipe(
        filter((message) => message.action == WebSocketActions.X01Create),
        tap(async (message) => {
          const socketMessage = message.message as CreateX01GameCommand;
          const meetingResponse = await firstValueFrom(
            meetingsRepository.createMeeting('First meeting test')
          );
          this.stateHooksService.handleGame(
            socketMessage.GameId!,
            async () => meetingResponse.data.id
          );
        })
      )
      .subscribe();
  }
  async getMeetingToken(meetingId: string): Promise<string> {
    const userName = sessionStorage.getItem('userName');
    const presetName = 'group_call_participant';

    if (!meetingId || !userName || !presetName) {
      throw Error('Unable to get dyte meeting token');
    }

    const response = await firstValueFrom(
      this.meetingsRepository.addParticipant(meetingId, userName, presetName)
    );

    return response.data.token;
  }
}
