import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FriendsNavComponent } from './friends-nav/friends-nav.component';

@Component({
  selector: 'app-friends-root',
  imports: [CommonModule, RouterModule, FriendsNavComponent],
  styleUrl: './friends-root.component.scss',
  templateUrl: './friends-root.component.html',
  standalone: true,
})
export class FriendsRootComponent implements AfterViewInit, OnDestroy {
  private resizeListener: (() => void) | null = null;

  ngAfterViewInit(): void {
    // Use setTimeout to ensure DOM is fully rendered
    setTimeout(() => {
      this.setHeight();
    }, 0);

    // Add resize listener
    this.resizeListener = () => this.setHeight();
    window.addEventListener('resize', this.resizeListener);
  }

  ngOnDestroy(): void {
    if (this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
  }

  private setHeight() {
    const navbar = document.getElementById('fdNavBar');
    const container = document.getElementById('friendsContainer');

    if (navbar && container) {
      const navbarHeight = navbar.offsetHeight;

      // Set the height of the friends container using calc
      console.log('[DEBUG] Setting height of friends container', navbarHeight);
      container.style.height = `calc(100vh - ${navbarHeight}px)`;
      container.style.minHeight = `calc(100vh - ${navbarHeight}px)`;
    } else {
      console.error('Navbar or Friends Container element not found');
    }
  }
}
