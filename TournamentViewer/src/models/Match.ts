import { Player } from "./Player";
import { Round } from "./Round";
import { Song } from "./Song";

export interface Match {
    id: number;
    name: string;
    players: Player[];
    songs: Song[];
    rounds: Round[];
}