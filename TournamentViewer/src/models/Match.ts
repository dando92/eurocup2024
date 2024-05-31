import { Player } from "./Player";
import { Round } from "./Round";
import { Song } from "./Song";

export interface Match {
  id: number;
  phaseId: number;
  name: string;
  subtitle: string;
  notes: string;
  isManualMatch: boolean;
  players: Player[];
  songs: Song[];
  rounds: Round[];
}
