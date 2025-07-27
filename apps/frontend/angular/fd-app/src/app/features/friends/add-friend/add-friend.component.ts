import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Router } from "@angular/router";
import { debounceTime, distinctUntilChanged, switchMap } from "rxjs";
import { UserSearchDto } from "../../../dtos/friend.dto";
import { FriendsService } from "../../../services/friends.service";
import { Store } from "@ngrx/store";

@Component({
  selector: "app-add-friend",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./add-friend.component.html",
})
export class AddFriendComponent implements OnInit {
  searchForm: FormGroup;
  searchResults: UserSearchDto[] = [];
  hasSearched = false;
  sendingRequest: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private friendsService: FriendsService,
    private router: Router,
    private store: Store
  ) {
    this.searchForm = this.formBuilder.group({
      searchQuery: ["", [Validators.required, Validators.minLength(2)]],
    });
  }

  ngOnInit(): void {
    // Set up search with debounce
    this.searchForm
      .get("searchQuery")
      ?.valueChanges.pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((query) => {
          if (query && query.length >= 2) {
            return this.friendsService.searchUsers(query);
          }
          return [];
        })
      )
      .subscribe((results) => {
        this.searchResults = results;
        this.hasSearched = true;
      });
  }

  onSearch(): void {
    if (this.searchForm.valid) {
      const query = this.searchForm.get("searchQuery")?.value;
      this.friendsService.searchUsers(query).subscribe({
        next: (results) => {
          this.searchResults = results;
          this.hasSearched = true;
        },
        error: (error) => {
          console.error("Error searching users:", error);
        },
      });
    }
  }

  sendFriendRequest(user: UserSearchDto): void {
    this.sendingRequest = user.UserId;

    this.friendsService.sendFriendRequest(user.UserId).subscribe({
      next: () => {
        console.log("Friend request sent successfully");
        // Update the user's status to show pending request
        const userIndex = this.searchResults.findIndex(
          (u) => u.UserId === user.UserId
        );
        if (userIndex !== -1) {
          this.searchResults[userIndex].HasPendingRequest = true;
        }
        this.sendingRequest = null;
      },
      error: (error) => {
        console.error("Error sending friend request:", error);
        this.sendingRequest = null;
      },
    });
  }

  goBack(): void {
    this.router.navigate(["/friends"]);
  }
}
