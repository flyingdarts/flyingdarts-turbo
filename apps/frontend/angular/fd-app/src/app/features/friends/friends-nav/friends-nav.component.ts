import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";

@Component({
  selector: "app-friends-nav",
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: "./friends-nav.component.html",
  styleUrl: "./friends-nav.component.scss",
})
export class FriendsNavComponent {
  pendingRequestsCount = 0; // This would be connected to the service in a real implementation
}
