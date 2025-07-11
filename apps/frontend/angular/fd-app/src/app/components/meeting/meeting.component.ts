import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DyteComponentsModule } from '@dytesdk/angular-ui-kit';
import DyteClient from '@dytesdk/web-core';
import { firstValueFrom } from 'rxjs';
import { MeetingsRepository } from 'src/app/repositories/meetings.repository';
import { MeetingService } from './services/meeting.service';

@Component({
  selector: 'app-meeting',
  templateUrl: './meeting.component.html',
  styleUrl: './meeting.component.scss',
  imports: [DyteComponentsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class MeetingComponent implements AfterViewInit {
  @Input({ required: true }) meetingId!: string | null;

  @ViewChild('localVideo')
  private localVideoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('opponentVideo')
  private opponentVideoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('opponentAudio')
  private opponentAudioElement!: ElementRef<HTMLAudioElement>;

  showSettings = false;

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly meetingsRepository: MeetingsRepository,
    private readonly meetingService: MeetingService
  ) {}

  openSettings() {
    this.showSettings = true;
  }

  closeSettings() {
    this.showSettings = false;
  }

  async ngAfterViewInit() {
    try {
      const userName = sessionStorage.getItem('userName')!;
      const meetingId =
        this.activatedRoute.snapshot.queryParamMap.get('meetingId')!;
      const meeting = await this.addParticipantToMeeting(meetingId, userName);

      this.meetingService.setupElements(
        this.localVideoElement.nativeElement,
        this.opponentAudioElement.nativeElement,
        this.opponentVideoElement.nativeElement
      );

      const dyteClient = await this.meetingService.create(meeting.data.token);

      this.setupDyteProvider(dyteClient);

      await dyteClient.join();
    } catch (error) {
      console.error('Error during initialization', error);
    }
  }

  private async addParticipantToMeeting(meetingId: string, userName: string) {
    return await firstValueFrom(
      this.meetingsRepository.addParticipant(
        meetingId,
        userName,
        'group_call_participant'
      )
    );
  }

  private setupDyteProvider(dyteClient: DyteClient) {
    const dyteProvider = document.getElementById('dyte-provider') as any;

    dyteProvider.meeting = dyteClient;
  }
}
