import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { firstValueFrom, map, Observable } from "rxjs";
import { AppStateActions, AppStateSelectors } from "src/app/state/app";
import { loadX01GameSettingsFromStorage } from "src/app/state/app/app.actions";
import { X01Settings } from "src/app/state/app/app.state";
import { FlyingdartsSdkService } from "src/sdk/flyingdarts-sdk-service";

@Component({
  selector: "app-home",
  imports: [CommonModule],
  templateUrl: "./home-lobby.component.html",
  standalone: true,
})
export class HomeLobbyComponent {
  x01SetsAndLegsText$: Observable<string>;

  constructor(
    private readonly flyingdartsSdkService: FlyingdartsSdkService,
    private readonly store: Store,
    private readonly router: Router
  ) {
    this.x01SetsAndLegsText$ = this.store
      .select(AppStateSelectors.selectX01GameSettings)
      .pipe(map((data) => `sets (${data.sets}) legs (${data.legs})`));

    this.store.dispatch(loadX01GameSettingsFromStorage());
  }

  async ngOnInit(): Promise<void> {
    this.flyingdartsSdkService.initSdk();
  }

  async goToSettings() {
    await this.router.navigate(["/", "settings"]);
  }
  async createGame() {
    const userId = await firstValueFrom(
      this.store.select(AppStateSelectors.selectUserId)
    );

    const gameSettings = JSON.parse(
      localStorage.getItem("X01GameSettings")!
    ) as X01Settings;

    this.flyingdartsSdkService.instance?.createGame(
      userId,
      gameSettings.sets,
      gameSettings.legs
    );

    this.store.dispatch(AppStateActions.setLoading({ loading: true }));
  }

  handleGameSettings(x01: { legs: number; sets: number }) {
    this.store.dispatch(AppStateActions.setX01GameSettings({ x01 }));
  }
}
