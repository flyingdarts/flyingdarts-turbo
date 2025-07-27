import { CommonModule } from "@angular/common";
import { Component, Input, forwardRef } from "@angular/core";
import { Observable } from "rxjs";

export enum NextPlayerType {
  Nobody,
  Me,
  Other,
}

@Component({
  selector: "app-next-player-status",
  standalone: true,
  imports: [CommonModule, forwardRef(() => NextPlayerStatusComponent)],
  template: `
    <app-next-player-status-ui
      [nextPlayerType$]="nextPlayerType$ | async"
    ></app-next-player-status-ui>
  `,
})
export class NextPlayerStatusContainerComponent {
  @Input() nextPlayerType$!: Observable<NextPlayerType>;
}

@Component({
  selector: "app-next-player-status-ui",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="next-player-status" [class]="getStatusClass()">
      <span class="status-icon">
        <i [class]="getStatusIcon()"></i>
      </span>
      <span class="status-text">{{ getStatusText() }}</span>
    </div>
  `,
  styles: [
    `
      .next-player-status {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.25rem 0.5rem;
        border-radius: 0.25rem;
        font-size: 0.875rem;
        font-weight: 500;
        transition: all 0.2s ease;
      }

      .next-player-status.nobody {
        background-color: #f8f9fa;
        color: #6c757d;
        border: 1px solid #dee2e6;
      }

      .next-player-status.me {
        background-color: #d4edda;
        color: #155724;
        border: 1px solid #c3e6cb;
      }

      .next-player-status.other {
        background-color: #fff3cd;
        color: #856404;
        border: 1px solid #ffeaa7;
      }

      .status-icon {
        display: flex;
        align-items: center;
      }

      .status-text {
        white-space: nowrap;
      }
    `,
  ],
})
export class NextPlayerStatusComponent {
  @Input() nextPlayerType$!: Observable<NextPlayerType>;
  @Input() currentType: NextPlayerType = NextPlayerType.Nobody;

  NextPlayerType = NextPlayerType; // Make enum available in template

  getStatusClass(): string {
    switch (this.currentType) {
      case NextPlayerType.Nobody:
        return "nobody";
      case NextPlayerType.Me:
        return "me";
      case NextPlayerType.Other:
        return "other";
      default:
        return "nobody";
    }
  }

  getStatusIcon(): string {
    switch (this.currentType) {
      case NextPlayerType.Nobody:
        return "bi bi-pause-circle";
      case NextPlayerType.Me:
        return "bi bi-person-check-fill";
      case NextPlayerType.Other:
        return "bi bi-person-fill";
      default:
        return "bi bi-pause-circle";
    }
  }

  getStatusText(): string {
    switch (this.currentType) {
      case NextPlayerType.Nobody:
        return "Waiting...";
      case NextPlayerType.Me:
        return "Your turn";
      case NextPlayerType.Other:
        return "Opponent's turn";
      default:
        return "Unknown";
    }
  }
}
