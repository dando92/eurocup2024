import { Standing } from "./Standing";
import { Song } from "./Song";

export interface Round {
  id: number;
  matchId: number;
  standings: Standing[];
  song: Song;
}
