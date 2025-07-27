import { CommonModule } from "@angular/common";
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { Store } from "@ngrx/store";
import { map, Observable } from "rxjs";
import { GameStateSelectors } from "src/app/state/game";

@Component({
  selector: "app-game-input-ui",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./game-input.component.html",
  styleUrls: ["./game-input.component.scss"],
})
export class GameInputComponent {
  @ViewChild("hiddenInputField")
  hiddenInputField!: ElementRef<HTMLInputElement>;

  focusKeyboardInput() {
    // Focus the hidden input field to capture keyboard events
    this.hiddenInputField.nativeElement.focus();
  }

  handleKeyboardInput($event: KeyboardEvent) {}

  @Input({ required: true }) shouldDisableControls!: boolean;
  @Input({ required: true }) canCheckOut!: boolean | null;

  @Input() mode: "light" | "dark" | null = "dark";

  readonly input$: Observable<string> = this.store
    .select(GameStateSelectors.selectGameInput)
    .pipe(map((x) => x ?? ""));

  constructor(private readonly store: Store) {}

  ngAfterViewInit() {
    this.hiddenInputField.nativeElement.focus();

    document.addEventListener("keydown", ($event: KeyboardEvent) => {
      if (this.shouldDisableControls) {
        return;
      }

      switch ($event.type) {
        case "keydown": {
          if (!isNaN(Number($event.key))) {
            // If the key is a number, emit it as input
            this.inputScoreHandler.emit($event.key);
          } else if ($event.key === "Enter") {
            // If the key is Enter, emit the input score
            this.okHandler.emit();
          } else if ($event.key === "Backspace") {
            // If the key is Escape, clear the score
            this.clearScoreHandler.emit();
            this.hiddenInputField.nativeElement.value = "";
          }
          break;
        }
      }
    });
  }

  @Output() inputScoreHandler = new EventEmitter<string>();
  @Output() clearScoreHandler = new EventEmitter<void>();
  @Output() noScoreHandler = new EventEmitter<void>();
  @Output() checkHandler = new EventEmitter<void>();
  @Output() okHandler = new EventEmitter<void>();
  @Output() quickScoreHandler = new EventEmitter<number>();

  clearScore() {
    this.clearScoreHandler.emit();
  }

  check() {
    this.checkHandler.emit();
  }

  inputScore(score: number) {
    this.inputScoreHandler.emit(score.toString());
  }

  noScore() {
    this.noScoreHandler.emit();
  }
  ok() {
    this.okHandler.emit();
  }

  quickScore(score: number) {
    this.quickScoreHandler.emit(score);
  }
}
