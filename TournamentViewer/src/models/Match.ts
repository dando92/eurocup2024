import { Player } from "./Player";
import { Song } from "./Song";

export interface Match {
    id: number;
    name: string;
    phaseId: number;
    players: Player[];
    songs: Song[];
}