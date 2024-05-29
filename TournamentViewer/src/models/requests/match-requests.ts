export interface CreateMatchRequest {
  divisionId: number;
  phaseId: number;
  matchName: string;
  group: string;
  levels: string;
  songIds: number[];
  playerIds: number[];
}

export interface SetActiveMatchRequest {
  divisionId: number;
  phaseId: number;
  matchId: number;
}
