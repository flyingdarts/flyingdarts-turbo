import { DartDto } from './dtos/DartDto';
import { PlayerDto } from './dtos/PlayerDto';
import { GameDto } from './dtos/GameDto';

export class Metadata {
  Game!: GameDto;
  Players!: PlayerDto[];
  Darts!: Record<string, DartDto[]>;
  NextPlayer!: string;
  WinningPlayer!: string;
}
