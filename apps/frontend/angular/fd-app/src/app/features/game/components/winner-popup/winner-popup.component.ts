import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-winner-popup',
  templateUrl: './winner-popup.component.html',
  styleUrls: ['./winner-popup.component.scss'],
  standalone: true,
  imports: [CommonModule],
})
export class WinnerPopupComponent implements OnInit {
  @Input({ required: true }) winnerText?: string | null | undefined;
  @Input({ required: true }) winnerName?: string | null | undefined;

  @Output() handleCloseEvent = new EventEmitter<void>();
  ngOnInit(): void {
    this.setHeight();
  }

  close() {
    this.handleCloseEvent.emit();
  }

  haveWinner(winnerName: string | null | undefined): boolean {
    return (
      winnerName !== undefined &&
      winnerName !== null &&
      winnerName.trim() !== ''
    );
  }

  private setHeight() {
    const navbar = document.getElementById('fdNavBar');
    const container = document.getElementById('winnerPopupOverlay');

    if (navbar && container) {
      const navbarHeight = navbar.offsetHeight;
      container.style.height = `calc(100% - ${navbarHeight}px)`;
      container.style.display = 'none';
    } else {
      console.error(
        'Navbar or Home Container element not found',
        navbar,
        container
      );
    }
  }
}
