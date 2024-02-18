import { AfterViewInit, Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { isNullOrUndefined } from 'src/app/app.component';
import { X01Store } from 'src/app/games/x01/x01.store';
import { WebSocketActions } from 'src/app/infrastructure/websocket/websocket.actions.enum';
import { WebSocketService } from 'src/app/infrastructure/websocket/websocket.service';
import { X01ApiService } from 'src/app/services/x01-api.service';

@Component({
  selector: 'app-video',
  standalone: true,
  imports: [],
  templateUrl: './video.component.html',
  styleUrls: ['./video.component.scss']
})
export class VideoComponent implements OnInit, OnChanges, AfterViewInit {
  @ViewChild('localVideo') localVideo!: ElementRef;
  @ViewChild('remoteVideo') remoteVideo!: ElementRef;
  @Input() nextPlayer!: string;
  
  public showPip = true;

  private localStream!: MediaStream;
  private peerConnection?: RTCPeerConnection;
  private opponent!: string;
  private player!: string;

  constructor(private apiService: X01ApiService, private x01Store: X01Store, private webSocketService: WebSocketService) { }
  ngAfterViewInit(): void {
    this.x01Store.select(x=>x.currentPlayer).subscribe(player => {
      if (!isNullOrUndefined(player) && player != "") { 
        this.connect();
      }
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (isNullOrUndefined(this.player) || isNullOrUndefined(this.opponent)) {
      return;
    }
    if (changes['nextPlayer'] && !isNullOrUndefined(changes['nextPlayer'].currentValue)) {
      this.handleNextPlayerChange(changes['nextPlayer'].currentValue);
    }
  }

  handleNextPlayerChange(player: string) {
    if (player != '') {
      this.switchStreams();
    }
  }

  switchStreams() {
    if (this.localVideo.nativeElement.srcObject && this.remoteVideo.nativeElement.srcObject) {

      var stream_a = this.localVideo.nativeElement.srcObject;
      var stream_b = this.remoteVideo.nativeElement.srcObject;
      this.localVideo.nativeElement.srcObject = stream_b;
      this.remoteVideo.nativeElement.srcObject = stream_a;
    } 
  }

 async ngOnInit() {
    var stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
    this.localVideo.nativeElement.srcObject = stream;
    this.localStream = stream;

    this.peerConnection = new RTCPeerConnection();
    this.localStream.getTracks().forEach(track => {
      this.peerConnection?.addTrack(track, this.localStream);
    })

    this.peerConnection.ontrack = event => {
      if (event.streams && event.streams[0]) {
        const remoteStream = event.streams[0];
        this.remoteVideo.nativeElement.srcObject = remoteStream;
      }
    }

    this.peerConnection.onicecandidate = event => {
      if (event.candidate) {
        var toUser = this.x01Store.getOpponentId();
        this.sendCandidate(event.candidate, toUser)
      }
    }
    
    this.webSocketService.getMessages().subscribe((x) => {
      switch (x.action) {
        case WebSocketActions.X01WebRTC:
          var fromUser: string = (x.message as any).FromUser;
          var toUser: string = (x.message as any).ToUser;
          const data = {
            type: (x.message as any).Type,
            sdp: (x.message as any).Sdp
          }
          if (data.type == "offer") {
            this.handleReceivedOffer(data, fromUser);
          } else {
            this.handleReceivedAnswer(data, toUser);
          }
          break;
        case WebSocketActions.X01WebRTCCandidate:
          let candidate: RTCIceCandidate = JSON.parse((x.message as any).Candidate);
          this.handleNewIceCandidate(candidate);
          break;
      }
    });
    
    this.x01Store.select(x=>x.player).subscribe(player => {
      this.player = player.id;
    })

    this.x01Store.select(x=>x.opponent).subscribe(player => {
      this.opponent = player.id;
    })
  }
  handleNewIceCandidate(candidate: RTCIceCandidate) {
    // console.log('Handling candidate', candidate)
    this.addIceCandidateWithRetry(candidate);
  }

  handleReceivedAnswer(answer: RTCSessionDescriptionInit, fromUser: string) {
    this.peerConnection?.setRemoteDescription(new RTCSessionDescription(answer))
      .catch(error => {
        console.error('Error setting remote description from answer:', error);
              // Retry signaling after a delay, or reset the whole process based on your app's logic:
      setTimeout(() => {
        this.retrySignalingProcess();
      }, 5000);
      });
  }
  retrySignalingProcess() {
    // Your logic for restarting or resetting the signaling process goes here
    // E.g., create a new offer and reinitiate the signaling process
  
    // Notify users about the retry
    // this.notifyUsers("We encountered an issue with your video call. Retrying...");
  
    // Clean up if needed
    this.cleanupPeerConnection();
  
    // Start fresh
    this.connect();
  }

  cleanupPeerConnection() {
    // Close and nullify the peer connection if needed
    if (this.peerConnection) {
      this.peerConnection.close();
      this.peerConnection = undefined;
    }
  }

  handleReceivedOffer(offer: RTCSessionDescriptionInit, fromUser: string) {
    const remoteDesc = new RTCSessionDescription(offer);
    this.peerConnection?.setRemoteDescription(remoteDesc)
      .then(() => this.peerConnection!.createAnswer())
      .then(answer => this.peerConnection!.setLocalDescription(answer))
      .then(() => {
        var toUser = this.x01Store.getPlayerId();
        this.sendAnswer(this.peerConnection!.localDescription!, toUser, fromUser);
      }).catch(error => {
        console.error('Error during offer-answer handling:', error);
        setTimeout(() => {
          this.retrySignalingProcess();
        }, 5000);
      });
  }
  addIceCandidateWithRetry(candidate: RTCIceCandidate, retries = 3) {
    this.peerConnection?.addIceCandidate(candidate).catch(error => {
      if (retries > 0) {
        console.warn(`Retry ${4 - retries} failed, retrying...`);
        setTimeout(() => this.addIceCandidateWithRetry(candidate, retries - 1), 1000);
      } else {
        console.error('Final attempt to add ICE candidate failed:', error);
        // Handle the permanent failure case here.
      }
    });
  }
connect(): void {
  console.log('Making the offer');
  this.peerConnection?.createOffer().then(offer => {
    this.peerConnection!.setLocalDescription(offer).then(() => {
      const toUser = this.x01Store.getOpponentId();
      const fromUser = this.x01Store.getPlayerId();
      this.sendOffer(offer, toUser, fromUser);
    }).catch(error => {
      console.error('Error during setLocalDescription:', error);
    });
  }).catch(error => {
    console.error('Error during createOffer:', error);
  });
}

  sendAnswer(answer: RTCSessionDescriptionInit, toUser: string, fromUser: string) {
    // console.log("sending answer", answer);
    this.apiService.webrtc(answer.type, answer.sdp!, toUser, fromUser);
  }
  sendOffer(offer: RTCSessionDescriptionInit, toUser: string, fromUser: string) {
    console.log("sending offer", offer)
    this.apiService.webrtc(offer.type, offer.sdp!, toUser, fromUser);
  }
  sendCandidate(candidate: RTCIceCandidate, toUser: string) {
    // console.log("sending candidate", candidate);
    this.apiService.webrtccandidate(JSON.stringify(candidate), toUser);
  }
}

export interface WebRPCVideoCommand {
  sdp: string;
  type: string;
  toUser: string;
  fromUser: string;
}

export interface WebRPCCandidateVideoCommand {
  toUser: string;
  candidate: string;
}