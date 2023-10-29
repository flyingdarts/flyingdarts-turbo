import { Injectable } from "@angular/core";
import { CreateRoomCommand } from "../requests/CreateRoomCommand";
import { WebSocketActions } from "../infrastructure/websocket/websocket.actions.enum";
import { WebSocketMessage } from "../infrastructure/websocket/websocket.message.model";
import { JoinGameCommand } from "../requests/JoinGameCommand";
import { WebSocketService } from "../infrastructure/websocket/websocket.service";

@Injectable({ providedIn: 'root' })
export class ApiService {
    constructor(private webSocketService: WebSocketService) {

    }

    roomsOnJoin(gameId: string, playerId: string, playerName: string) {
        var message: JoinGameCommand = {
            GameId: gameId,
            PlayerId: playerId,
            PlayerName: playerName
        };
        let body: WebSocketMessage<JoinGameCommand> = {
            action: WebSocketActions.X01Join,
            message: message
        };
        console.log(body);
        this.webSocketService.postMessage(JSON.stringify(body));
    }

    roomsOnCreate(roomId: string) {
        var message: CreateRoomCommand = {
            RoomId: roomId
        };
        let body: WebSocketMessage<CreateRoomCommand> = {
            action: WebSocketActions.RoomsOnCreate,
            message: message
        };
        console.log(body);
        this.webSocketService.postMessage(JSON.stringify(body));
    }
}