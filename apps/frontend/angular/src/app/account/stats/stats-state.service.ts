import { Injectable } from '@angular/core';

// Assuming StatsDart is defined elsewhere, import it
// If StatsDart needs to be defined, provide a basic structure below
export class StatsDart {
  // Example properties, adjust according to actual requirements
  id!: number;
  score!: number;
  // Add other properties as needed
}

@Injectable({
  providedIn: 'root'
})
export class StatsStateService {
  private storage = localStorage;

  // Key used to store and retrieve the StatsDart array from localStorage
  private readonly statsDartKey = 'StatsStateService.Darts';

  // Clears all stored StatsDart objects and other related data
  public clear(): void {
    this.storage.removeItem('StatsStateService.LastUpdated');
    this.storage.removeItem(this.statsDartKey);
  }

  // Retrieves the stored array of StatsDart objects
  public get darts(): Array<StatsDart> {
    const serializedDarts = this.storage.getItem(this.statsDartKey);
    if (serializedDarts) {
      return JSON.parse(serializedDarts);
    }
    // Return an empty array if there's nothing stored
    return [];
  }

  // Saves the provided array of StatsDart objects to localStorage
  public set darts(value: Array<StatsDart>) {
    this.storage.setItem(this.statsDartKey, JSON.stringify(value));
    // Optionally, update the last updated timestamp or other relevant metadata
    this.storage.setItem('StatsStateService.LastUpdated', new Date().toISOString());
  }
}