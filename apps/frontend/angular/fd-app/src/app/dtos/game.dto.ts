import { X01SettingsDto } from "./x01-settings.dto";

export class GameDto {
  Id!: string;
  Type!: number;
  Status!: number;
  PlayerCount!: number;
  X01!: X01SettingsDto;
}
