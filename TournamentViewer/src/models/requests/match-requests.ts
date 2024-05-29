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

export interface AddSongToMatchRequest {
  divisionId: number;
  phaseId: number;
  matchId: number;
  group?: string;
  level?: string;
  songId?: number;
}

export interface AddStandingToMatchRequest {
  playerId: number;
  songId: number;
  percentage: number;
  isFailed: boolean;
}
