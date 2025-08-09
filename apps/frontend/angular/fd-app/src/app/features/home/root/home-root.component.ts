import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home-root',
  imports: [CommonModule, RouterModule],
  styleUrl: './home-root.component.scss',
  templateUrl: './home-root.component.html',
  standalone: true,
})
export class HomeRootComponent implements OnInit {
  ngOnInit(): void {
    this.setHeight();
  }

  private setHeight() {
    const navbar = document.getElementById('fdNavBar');
    const homecontainer = document.getElementById('homeContainer');

    if (navbar && homecontainer) {
      const navbarHeight = navbar.offsetHeight;

      // Set the height of the home container using calc
      homecontainer.style.height = `calc(100% - ${navbarHeight}px)`;
    } else {
      console.error('Navbar or Home Container element not found');
    }
  }
}
