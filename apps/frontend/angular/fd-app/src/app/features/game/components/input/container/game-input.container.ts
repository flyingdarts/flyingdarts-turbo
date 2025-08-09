import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppStateSelectors } from 'src/app/state/app';
import { GameInputComponent } from '../presentation/game-input.component';

@Component({
  selector: 'app-game-input',
  standalone: true,
  imports: [CommonModule, GameInputComponent],
  templateUrl: './game-input.container.html',
})
export class GameInputContainerComponent {
  themeMode$: Observable<'light' | 'dark'>;

  constructor(private readonly store: Store) {
    this.themeMode$ = this.store.select(AppStateSelectors.selectThemeMode);
    this.themeMode$.subscribe();
  }

  @Input({ required: true }) shouldDisableControls!: boolean;
  @Input({ required: true }) canCheckOut!: boolean | null;

  @Output() inputScoreHandler = new EventEmitter<string>();
  @Output() clearScoreHandler = new EventEmitter<void>();
  @Output() noScoreHandler = new EventEmitter<void>();
  @Output() checkHandler = new EventEmitter<void>();
  @Output() okHandler = new EventEmitter<void>();
  @Output() quickScoreHandler = new EventEmitter<number>();

  handleInputScore(score: string) {
    this.inputScoreHandler.emit(score);
  }
  handleClearScore() {
    this.clearScoreHandler.emit();
  }
  handleNoScore() {
    this.noScoreHandler.emit();
  }
  handleCheck() {
    this.checkHandler.emit();
  }
  handleOk() {
    this.okHandler.emit();
  }
  handleQuickScore(score: number) {
    this.quickScoreHandler.emit(score);
  }
}
