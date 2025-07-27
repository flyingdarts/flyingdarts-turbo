import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { FriendsRootComponent } from "./friends-root.component";
import { FriendsListComponent } from "./friends-list/friends-list.component";
import { AddFriendComponent } from "./add-friend/add-friend.component";
import { FriendRequestsComponent } from "./friend-requests/friend-requests.component";

const routes: Routes = [
  {
    path: "",
    component: FriendsRootComponent,
    children: [
      {
        path: "",
        component: FriendsListComponent,
      },
      {
        path: "add",
        component: AddFriendComponent,
      },
      {
        path: "requests",
        component: FriendRequestsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FriendsRoutingModule {}
