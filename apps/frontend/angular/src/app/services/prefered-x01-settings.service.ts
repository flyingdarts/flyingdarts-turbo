import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PreferedX01SettingsService {
  private storage = localStorage;

  public clear(): void {
    this.storage.removeItem('PreferedSettings.X01Sets');
    this.storage.removeItem('PreferedSettings.X01Legs');
  }

  public get preferedX01Sets(): number {
    const key = "PreferedSettings.X01Sets";
    return Number(this.storage.getItem(key));
  }

  public get preferedX01Legs(): number {
    const key = "PreferedSettings.X01Legs";
    return Number(this.storage.getItem(key));
  }

  public set preferedX01Sets(sets: number) {
    const key = "PreferedSettings.X01Sets";
    this.storage.setItem(key, sets.toString());
  }

  public set preferedX01Legs(legs: number) {
    const key = "PreferedSettings.X01Legs";
    this.storage.setItem(key, legs.toString());
  }
}
