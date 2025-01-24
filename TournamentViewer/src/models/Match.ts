import { Player } from "./Player";
import { Round } from "./Round";

export interface Match {
  id: number;
  phaseId: number;
  name: string;
  subtitle: string;
  notes: string;
  isManualMatch: boolean;
  players: Player[];
  rounds: Round[];
}
