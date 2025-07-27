import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Store } from "@ngrx/store";
import { firstValueFrom } from "rxjs";
import { MeetingComponent } from "src/app/components/meeting/meeting.component";
import { GameStateSelectors } from "src/app/state/game";
import { ResponsiveService } from "src/app/services/responsive/responsive.service";
import { GameInputContainerComponent } from "../components/input/container/game-input.container";
import { GameStatsContainerComponent } from "../components/stats/container/game-stats.container";
import { WinnerPopupComponent } from "../components/winner-popup/winner-popup.component";

@Component({
  selector: "app-game-ui",
  standalone: true,
  imports: [
    CommonModule,
    GameStatsContainerComponent,
    GameInputContainerComponent,
    MeetingComponent,
    WinnerPopupComponent,
  ],
  templateUrl: "./game.component.html",
  styleUrl: "./game.component.scss",
})
export class GameComponent {
  @Input() isPlayerTurn: boolean | null = null;
  @Input() shouldDisableControls: boolean = true;
  @Input() canCheckOut: boolean | null = false;
  @Input() showGameInput: boolean | null = true;
  @Input() winnerName: string | null | undefined;
  @Input() winnerText: string | null | undefined;
  @Input() showMeeting: boolean = true;

  @Output() handleInputScore = new EventEmitter<string>();
  @Output() handleNoScore = new EventEmitter<number>();
  @Output() handleOk = new EventEmitter<{
    score: number;
    updatedScore: number;
  }>();
  @Output() handleCheck = new EventEmitter<number>();
  @Output() handleClear = new EventEmitter<string>();
  @Output() handleQuickScore = new EventEmitter<{
    score: number;
    updatedScore: number;
  }>();

  @Output() handleClosePopup = new EventEmitter<string>();

  constructor(
    private readonly store: Store,
    private readonly responsiveService: ResponsiveService
  ) {}

  // Responsive helper methods
  isDesktopLayout(): boolean {
    return this.responsiveService.shouldShowDesktopLayout();
  }

  isMobileLayout(): boolean {
    return this.responsiveService.shouldShowMobileLayout();
  }

  isMobileDevice(): boolean {
    return this.responsiveService.isMobileDevice();
  }

  isTabletDevice(): boolean {
    return this.responsiveService.isTabletDevice();
  }

  isLandscapeOrientation(): boolean {
    return this.responsiveService.isLandscapeOrientation();
  }

  // Method to determine if we should show the video (when it's not the player's turn)
  shouldShowMeeting(): boolean {
    if (this.isDesktopLayout()) {
      return true; // Always show on desktop/tablet landscape
    }

    // On mobile/tablet portrait, show video when it's not my turn
    return !(this.showGameInput ?? false);
  }

  // Method to determine if we should show the game controls
  shouldShowGameControls(): boolean {
    if (this.isDesktopLayout()) {
      return true; // Always show on desktop/tablet landscape
    }

    // On mobile/tablet portrait, show controls when it's my turn
    return this.showGameInput ?? false;
  }

  // Method to determine if we need to show the fallback layout
  shouldShowFallbackLayout(): boolean {
    return !this.isDesktopLayout() && !this.isMobileLayout();
  }

  public closePopup() {
    this.handleClosePopup.emit();
  }
  public inputScore(score: string) {
    this.handleInputScore.emit(score);
  }
  public async noScore() {
    const currentScore = await firstValueFrom(
      this.store.select(GameStateSelectors.selectPlayerScore)
    );

    this.shouldDisableControls = true;
    this.handleNoScore.emit(currentScore);
  }
  public async ok() {
    const [currentScore, input] = await Promise.all([
      firstValueFrom(this.store.select(GameStateSelectors.selectPlayerScore)),
      firstValueFrom(this.store.select(GameStateSelectors.selectGameInput)),
    ]);

    const score = Number(input);

    const bogeyScores = [159, 162, 163, 165, 166, 168, 169];
    const updatedScore = currentScore - score;

    if (updatedScore === 0 && (score > 170 || bogeyScores.includes(score))) {
      console.warn(
        "Invalid operation: Updated score cannot be zero for this input."
      );
      return;
    }

    if (updatedScore < 0) {
      return;
    }

    this.shouldDisableControls = true;
    this.handleOk.emit({ score, updatedScore });
  }
  public async check() {
    const currentScore = await firstValueFrom(
      this.store.select(GameStateSelectors.selectPlayerScore)
    );
    const bogeyScores = [159, 162, 163, 165, 166, 168, 169];

    if (currentScore > 170 || bogeyScores.includes(currentScore)) {
      return;
    }

    this.shouldDisableControls = true;
    this.handleCheck.emit(currentScore);
  }
  public clear() {
    this.handleClear.emit();
  }

  public async quickScore(score: number) {
    const currentScore = await firstValueFrom(
      this.store.select(GameStateSelectors.selectPlayerScore)
    );

    const updatedScore = currentScore - score;
    if (updatedScore < 0) {
      return;
    }

    this.shouldDisableControls = true;
    this.handleQuickScore.emit({ score, updatedScore });
  }
}
