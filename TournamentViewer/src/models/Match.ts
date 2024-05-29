import { Player } from "./Player";
import { Round } from "./Round";
import { Song } from "./Song";

export interface Match {
  id: number;
  name: string;
  subtitle: string;
  notes: string;
  players: Player[];
  songs: Song[];
  rounds: Round[];
}
