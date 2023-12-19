import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { WebSocketActions } from 'src/app/infrastructure/websocket/websocket.actions.enum';
import { WebSocketService } from 'src/app/infrastructure/websocket/websocket.service';
import { HandleX01QueueCommand } from 'src/app/requests/HandleX01QueueCommand';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.scss']
})
export class QueueComponent implements OnInit {
  public queueName: string = '';

  constructor(
    private activatedRoute: ActivatedRoute,
    private webSocketService: WebSocketService,
    private router: Router,

    ) { }

  ngOnInit(): void {
    this.queueName = `${this.activatedRoute.snapshot.params['gameSettingsType']} Queue`
    this.webSocketService.getMessages().subscribe(x => {
      switch (x.action) {
        case WebSocketActions.X01Queue:
          this.router.navigate(['/', 'x01', (x.message as HandleX01QueueCommand).GameId]);
        break;
      }
    })
  }

}
