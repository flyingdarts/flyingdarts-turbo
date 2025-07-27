export interface Hooks {
  handleGame(gameId: string, meetingId: () => Promise<string> | string): void;
}
