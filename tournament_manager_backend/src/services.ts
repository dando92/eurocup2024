import { DivisionsService } from './services/divisions.service';
import { MatchesService } from './services/match.service';
import { PhasesService } from './services/phases.service';
import { PlayerService } from './services/players.service';
import { RoundsService } from './services/rounds.service';
import { TeamsService } from './services/teams.service';
import { TournamentsService } from './services/tournaments.service';
import { ScoresService } from './services/scores.service';
import { SongService } from './services/songs.service';
import { StandingsService } from './services/standing.service';


export const Services = [
    DivisionsService,
    MatchesService,
    PhasesService,
    PlayerService,
    RoundsService,
    ScoresService,
    SongService,
    StandingsService,
    TeamsService,
    TournamentsService
  ]