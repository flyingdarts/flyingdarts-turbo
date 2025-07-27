import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  ViewChild,
  OnDestroy,
} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DyteComponentsModule } from "@dytesdk/angular-ui-kit";
import DyteClient from "@dytesdk/web-core";
import { MeetingService } from "./services/meeting.service";

/**
 * Meeting component that supports video switching based on player turn.
 *
 * Features:
 * - Video switching: When isPlayerTurn is true, the player's video is shown in the main area
 *   and the opponent's video is shown in the corner. When false, the opposite occurs.
 * - Responsive design: Works on desktop/tablet layouts with keyboard on the left
 * - Visibility control: Can be hidden/shown using the isVisible input
 *
 * Usage:
 * <app-meeting
 *   [meetingId]="meetingId"
 *   [isVisible]="true"
 *   [isPlayerTurn]="isCurrentPlayerTurn">
 * </app-meeting>
 */
@Component({
  selector: "app-meeting",
  templateUrl: "./meeting.component.html",
  styleUrl: "./meeting.component.scss",
  imports: [DyteComponentsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class MeetingComponent implements AfterViewInit, OnDestroy {
  @Input({ required: true }) meetingId!: string | null;
  @Input() isVisible: boolean = true;
  /**
   * Controls which video is shown in the main video area.
   * - When true: Shows the player's own video in the main area and opponent's video in the corner
   * - When false: Shows the opponent's video in the main area and player's video in the corner
   */
  @Input() isPlayerTurn: boolean | null = null;

  @ViewChild("localVideo")
  private localVideoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild("opponentVideo")
  private opponentVideoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild("localVideoSmall")
  private localVideoSmallElement!: ElementRef<HTMLVideoElement>;
  @ViewChild("opponentVideoSmall")
  private opponentVideoSmallElement!: ElementRef<HTMLVideoElement>;
  @ViewChild("opponentAudio")
  private opponentAudioElement!: ElementRef<HTMLAudioElement>;

  showSettings = false;
  private dyteClient: DyteClient | null = null;
  private isJoining = false;
  private hasJoined = false;

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly meetingService: MeetingService
  ) {}

  openSettings() {
    this.showSettings = true;
  }

  closeSettings() {
    this.showSettings = false;
  }

  async ngAfterViewInit() {
    // Prevent multiple initialization attempts
    if (this.isJoining || this.hasJoined) {
      return;
    }

    try {
      this.isJoining = true;
      const meetingToken = sessionStorage.getItem("meetingToken")!;

      // Set up the main video elements
      this.meetingService.setupElements(
        this.localVideoElement.nativeElement,
        this.opponentAudioElement.nativeElement,
        this.opponentVideoElement.nativeElement
      );

      this.dyteClient = await this.meetingService.create(meetingToken);

      // Set up additional video elements for switching
      this.setupVideoSwitching(this.dyteClient);

      this.setupDyteProvider(this.dyteClient);

      await this.dyteClient.join();
      this.hasJoined = true;
    } catch (error) {
      console.error("Error during initialization", error);
      this.hasJoined = false;
    } finally {
      this.isJoining = false;
    }
  }

  ngOnDestroy() {
    if (this.dyteClient && this.hasJoined) {
      try {
        this.dyteClient.leave();
      } catch (error) {
        console.error("Error leaving meeting", error);
      }
    }
  }

  private setupVideoSwitching(dyteClient: DyteClient) {
    // Set up local video for both main and corner elements
    if (dyteClient.self.videoTrack) {
      this.attachVideoTrack(
        dyteClient.self.videoTrack,
        this.localVideoSmallElement.nativeElement
      );
    }

    // Set up opponent video for corner element
    dyteClient.participants.joined.on("videoUpdate", (participant) => {
      if (participant.videoTrack) {
        this.attachVideoTrack(
          participant.videoTrack,
          this.opponentVideoSmallElement.nativeElement
        );
      }
    });
  }

  private attachVideoTrack(track: MediaStreamTrack, element: HTMLVideoElement) {
    try {
      const stream = new MediaStream();
      stream.addTrack(track);
      element.srcObject = stream;
      element.onloadedmetadata = () => {
        element
          .play()
          .catch((error: unknown) =>
            console.error("Error playing media", error)
          );
      };
    } catch (error) {
      console.error("Error attaching video track", error);
    }
  }

  private setupDyteProvider(dyteClient: DyteClient) {
    const dyteProvider = document.getElementById("dyte-provider") as any;

    dyteProvider.meeting = dyteClient;
  }
}
