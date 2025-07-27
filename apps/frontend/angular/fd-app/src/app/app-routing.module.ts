import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GameContainerComponent } from "./features/game/container/game.container";
import { LoginComponent } from "./features/login/login.component";
import { AuthGuard } from "./guards/auth.guard";
import { SessionUserResolver } from "./resolvers/user-profile.resolver";

export const routes: Routes = [
  {
    path: "login",
    component: LoginComponent,
  },
  {
    path: "",
    canActivate: [AuthGuard],
    resolve: {
      sessionUserResolved: SessionUserResolver,
    },
    children: [
      {
        path: "",
        loadChildren: () =>
          import("./features/home/home.module").then((m) => m.HomeModule),
      },
      {
        path: "friends",
        loadChildren: () =>
          import("./features/friends/friends.module").then(
            (m) => m.FriendsModule
          ),
      },
      {
        path: "game/:gameId",
        component: GameContainerComponent,
      },
    ],
  },
  { path: "**", redirectTo: "", pathMatch: "full" }, // Redirect unknown paths to home
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
