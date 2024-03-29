import { Component, ElementRef, OnInit } from '@angular/core';
import { X01State, initialX01State } from './x01.state';
import { X01Store } from './x01.store';
import { Observable, first } from 'rxjs';
import { X01Input } from './x01.input';
import { WebSocketService } from 'src/app/infrastructure/websocket/websocket.service';
import { WebSocketActions } from 'src/app/infrastructure/websocket/websocket.actions.enum';
import { X01ApiService } from 'src/app/services/x01-api.service';
import { ActivatedRoute } from '@angular/router';
import { UserProfileStateService } from 'src/app/services/user-profile-state.service';
import { isNullOrUndefined } from 'src/app/app.component';
import { JitsiService } from 'src/app/services/jitsi.service';
import { Metadata } from './Metadata';
import { AnimationOptions } from 'ngx-lottie';
import { DartDto } from './dtos/DartDto';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-x01',
  templateUrl: './x01.component.html',
  styleUrls: ['./x01.component.scss'],
  providers: [X01Store],
})
export class X01Component implements OnInit {
  public title: string = '';
  public currentPlayerAnimation: AnimationOptions = {
    path: '/assets/animations/flyingdarts_icon.json',
    loop: true
  };
  public winnerAnimation: AnimationOptions = {
    path: '',
    loop: true
  }
  public input: X01Input = new X01Input(0, "");

  public vm$: Observable<X01State>;

  private gameId?: string;

  public clientId?: string;
  public winner?: string;
  public shouldDisableInput: boolean = false;
  public shouldDisableShortcuts: boolean = false;
  public currentPlayer?: string;
  public hasOpponent: boolean = false;

  constructor(
    private webSocketService: WebSocketService,
    private x01ApiService: X01ApiService,
    private route: ActivatedRoute,
    private userProfileService: UserProfileStateService,
    private jitsiService: JitsiService,
    private x01Store: X01Store
  ) {

    this.vm$ = this.x01Store.select(
    (state) => state
  );
   }

  async ngOnInit() {
    this.gameId = this.route.snapshot.paramMap.get('id')!;
    this.clientId = this.userProfileService.currentUserProfileDetails.UserId!;
    this.x01Store.setState(initialX01State);
    this.webSocketService.connected$.subscribe((connected) => {
      if (connected) {
        this.userProfileService.userName$.subscribe((userName) => {
          this.x01ApiService.joinGame(this.gameId!, this.clientId!, userName);
        });
      }
    });

    this.webSocketService.getMessages().subscribe((x) => {
      switch (x.action) {
        case WebSocketActions.X01Join:
        case WebSocketActions.X01Score:
          this.handleMetadata(x);
          break;
      }
    });
    this.setNavbarLink();
  }
  setNavbarLink() {
    let element = document.getElementById("copyLink");
    if (!isNullOrUndefined(element)) { 
      var copyString = 'Share this link (click to copy): ';
          copyString += window.location.toString().split('/')[window.location.toString().split('/').length-1];
          copyString += ' <i class="fa-regular fa-copy"></i>';
      element!.innerHTML = copyString;
      element!.onclick = this.copyLocationToClipboard
      element!.removeAttribute("href")
    }
  }
  private handleMetadata(message: any) {
    this.x01Store.setLoading(false);

    var metadata: Metadata = message.metadata;
    if (!isNullOrUndefined(metadata)) {
      // general stuff
      this.title = `Best of ${metadata.Game!.X01.Sets}/${metadata.Game!.X01.Legs}`;
      this.x01Store.setCurrentPlayer(metadata.NextPlayer);
      this.x01Store.setWinningPlayer(metadata.WinningPlayer);
      var player = metadata.Players.filter(x => x.PlayerId == this.clientId!)[0];
      this.x01Store.setPlayerId(player.PlayerId);
      this.x01Store.setPlayerName(player.PlayerName);
      this.x01Store.setPlayerCountry(player.Country);
      this.x01Store.setPlayerLegs(Number(player.Legs));
      this.x01Store.setPlayerSets(Number(player.Sets));
      if (metadata.Darts != null) {
        var playerDarts = metadata.Darts[this.clientId!];
        this.x01Store.setPlayerScore(metadata.Game.X01.StartingScore - this._sumOfHistory(playerDarts))
        this.x01Store.setPlayerHistory(playerDarts.map(x => x.Score));
      }
      if (Object.keys(metadata.Players).length == 2) {
        this.hasOpponent = true;
        var opponent = metadata.Players.filter(x => x.PlayerId != this.clientId!)[0];
        this.x01Store.setOpponentId(opponent.PlayerId);
        this.x01Store.setOpponentName(opponent.PlayerName);
        this.x01Store.setOpponentCountry(opponent.Country);
        this.x01Store.setOpponentLegs(Number(opponent.Legs));
        this.x01Store.setOpponentSets(Number(opponent.Sets));
        if (metadata.Darts != null) {
          var opponentDarts = metadata.Darts[opponent.PlayerId];
          this.x01Store.setOpponentScore(metadata.Game.X01.StartingScore - this._sumOfHistory(opponentDarts))
          this.x01Store.setOpponentHistory(opponentDarts.map(x => x.Score));
        }
      }
    } else {
      console.log("couldnt parse metadata");
    }
  }
  private _sumOfHistory(darts: DartDto[]): number {
    let sum = 0;
    for (let i = 0; i < darts.map(x => x.Score).length; i++) {
      sum += darts.map(x => x.Score)[i];
    }
    return sum;
  }
  public copyLink() {
    this.copyLocationToClipboard();
  }
  copyLocationToClipboard() {
    console.log('copy to clipboard')
    // Create a temporary input element to hold the URL
    const tempInput = document.createElement('input');
    
    // Append it to the body
    document.body.appendChild(tempInput);
    
    // Set its value to the current location's href
    tempInput.value = window.location.href;
    
    // Select the value
    tempInput.select();
    tempInput.setSelectionRange(0, 99999); // For mobile devices
    
    // Execute the copy command
    try {
      const successful = document.execCommand('copy');
      if (successful) {
        alert('URL copied to clipboard.');
      } else {
        alert('Failed to copy URL.');
      }
    } catch (err) {
      alert('Failed to copy URL. Error: ' + err);
    }
    
    // Remove the temporary input from the document
    document.body.removeChild(tempInput);
  }
  public sendScore() {
    this.x01Store.setLoading(true);
    this.x01Store.playerScore$.pipe(first()).subscribe((score: number) => {
      const newScore = score - this.input.Sum;

      if (newScore >= 0) {
        this.x01ApiService.score(
          this.gameId!,
          this.clientId!,
          newScore,
          this.input.Sum
        );
        this.shouldDisableInput = true;
      } else {
        // Handle invalid score here (e.g., show an error message)
        alert('Invalid score');
        return;
      }
    });
  }
  public resetScore() {
    this.input.reset();
  }
  public inputScore(score: number) {
    if ([26, 41, 45, 60, 85, 100].includes(score)) {
      if (this.input.Sum > 0) {
        alert('Please clear before using shortcut buttons');
        return;
      }
      this.input.reset();
      this.input.next(score);
      this.sendScore();
      this.input.reset();
      this.x01Store.setCurrentInput(this.input.getSum())
      return;
    }
    var nextInput = this.input.Input;
    nextInput += score;
    if (Number(nextInput) > 180) {
      return;
    }

    this.input.next(score);
    this.x01Store.setCurrentInput(this.input.getSum());
  }
  public noScore() {
    this.input.reset();
    this.sendScore();
    this.x01Store.setCurrentInput(this.input.getSum());
  }
  public ok() {
    this.sendScore();
    this.input.reset();
    this.x01Store.setCurrentInput(this.input.getSum())
  }
  public check() {
    this.input.reset();
    this.x01Store.setCurrentInput(this.input.getSum())
  }
  public clearScore() {
    this.input.reset();
    this.x01Store.setCurrentInput(this.input.getSum());
  }
}
