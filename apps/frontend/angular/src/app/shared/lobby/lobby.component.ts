import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from 'aws-amplify';
import { AnimationOptions } from 'ngx-lottie/lib/symbols';
import { FormControl, FormGroup } from '@angular/forms';
import { WebSocketService } from '../../infrastructure/websocket/websocket.service';
import { WebSocketActions } from '../../infrastructure/websocket/websocket.actions.enum';
import { MessageRequest } from '../../infrastructure/websocket/websocket.request.model';
import { WebSocketStatus } from '../../infrastructure/websocket/websocket.status.enum';
import { X01ApiService } from '../../services/x01-api.service';
import { JoinX01QueueCommand } from '../../requests/JoinX01QueueCommand';
import { AppStore } from 'src/app/app.store';
import { UserProfileStateService } from '../../services/user-profile-state.service';
import { Observable } from 'rxjs';
const { v4: uuidv4 } = require('uuid');

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit {
  public loading$: Observable<boolean> = this.store.select(x=>x.loading);

  public messages: MessageRequest[] = [];
  public webSocketStatus: WebSocketStatus = WebSocketStatus.Unknown
  public clientId: string = ""
  public playerId: string = "";
  public lottieOptions: AnimationOptions = {
    path: '/assets/animations/play.json'
  };
  public shouldHideLoader: boolean = true;
  public messageForm = new FormGroup({
    message: new FormControl(''),
  });
  constructor(
    private userProfileService: UserProfileStateService,
    private x01ApiService: X01ApiService,
    private router: Router,
    private webSocketService: WebSocketService,
    private store: AppStore
  ) {
    
  }

  ngOnInit(): void {
    this.webSocketService.getMessages().subscribe(x=> {
      switch(x.action) {
        case WebSocketActions.Connect:
          this.webSocketStatus = WebSocketStatus.Connected
          break;
        case WebSocketActions.Default:
          this.messages.push(x.message! as MessageRequest)
          break;
        case WebSocketActions.Disconnect:
          this.webSocketStatus = WebSocketStatus.Disconnected
          break;
        case WebSocketActions.X01JoinQueue:
          this.shouldHideLoader = !this.shouldHideLoader;
          this.router.navigate(['/', 'x01', (x.message as JoinX01QueueCommand).GameId]);
        break;
      }
    })
    this.store.setProfile(this.userProfileService.currentUserProfileDetails);
    this.clientId = this.userProfileService.currentUserProfileDetails.UserId!;
  }

  public joinX01Queue() {
    this.shouldHideLoader = !this.shouldHideLoader;
    this.x01ApiService.joinQueue(this.clientId);
  }

  public sendMessage() {
    console.log("Sending message", {
      date: new Date(),
      message: this.messageForm.value.message,
      owner: this.clientId,
    });
    this.webSocketService.postMessage(JSON.stringify({
      action: "message",
      message: {
        date: new Date(),
        message: this.messageForm.value.message,
        owner: this.clientId,
      }
    }))
  }
}
