import { X01SettingsDto } from './X01SettingsDto';

export class GameDto {
  Id!: string;
  Type!: number;
  Status!: number;
  PlayerCount!: number;
  X01!: X01SettingsDto;
}
