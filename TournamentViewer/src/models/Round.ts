import { Standing } from "./Standing";

export interface Round {
    id: number;
    matchId: number;
    standings: Standing[];
}