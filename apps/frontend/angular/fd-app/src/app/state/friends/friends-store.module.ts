import { NgModule } from "@angular/core";
import { StoreModule } from "@ngrx/store";
import { friendsReducer } from "./friends.reducer";

@NgModule({
  imports: [StoreModule.forFeature("friendsState", friendsReducer)],
})
export class FriendsStoreModule {}
