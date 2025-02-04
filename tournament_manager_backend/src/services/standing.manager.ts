import { Injectable, Inject } from "@nestjs/common";
import { PlayerService, SongService, StandingsService, ScoresService } from "src/crud/services";
import { LiveScore } from "../gateways/live.score.gateway"
import { CreateScoreDto, CreateStandingDto, UpdateStandingDto } from "src/crud/dtos";
import { TournamentCache } from "./tournament.cache";
import { Standing, Player, Round } from "src/crud/entities";
import { MatchGateway } from '../gateways/match.gateway';
import { ScoringSystemProvider } from './IScoringSystem'

@Injectable()
export class StandingManager {
    constructor(
        @Inject()
        private readonly standingService: StandingsService,
        @Inject()
        private readonly scoresService: ScoresService,
        @Inject()
        private readonly songService: SongService,
        @Inject()
        private readonly playerService: PlayerService,
        @Inject()
        private readonly tournamentCache: TournamentCache,
        @Inject()
        private readonly matchHub: MatchGateway,
        @Inject()
        private readonly scoringSystemProvider: ScoringSystemProvider
    ) { }

    async AddScore(score: CreateScoreDto) {

        const actualScoreEntity = await this.scoresService.create(score)

        const activeMatch = await this.tournamentCache.GetActiveMatch();
        const round = activeMatch.rounds.find(async round=> round.song.id === score.songId);

        if(!activeMatch) {
            //TODO: Log score added but no active match found
            return;
        }

        if(!round) {
            //TODO: Log socre added but no round found in active match
            return;
        }
        
        const newStanding = new CreateStandingDto();
        
        newStanding.roundId = activeMatch.id;
        newStanding.scoreId = actualScoreEntity.id

        await this.standingService.create(newStanding);
        const activePlayerInRound = this.CountActivePlayersInRound(activeMatch.players, round);
        
        if(round.standings.length >= activePlayerInRound) {
            const scoreSystem = this.scoringSystemProvider.getScoringSystem(activeMatch.scoringSystem);
            scoreSystem.recalc(round.standings);
            await this.recalc(round.standings);
        }

        this.matchHub.OnMatchUpdate(activeMatch);
    }

    async AddLiveScore(score: LiveScore) {
        const song = await this.songService.findByName(score.song);

        if (!song) {
            throw new Error(`Song with title ${score.song} not found`);
        }

        const player = await this.playerService.findByName(score.playerName);

        if (!player) {
            throw new Error(`Player with name ${score.playerName} not found`)
        }
        
        const newScore = new CreateScoreDto();

        newScore.playerId = player.id;
        newScore.songId = song.id;
        newScore.percentage = parseFloat(score.formattedScore);
        newScore.isFailed = score.isFailed;
        
        return await this.AddScore(newScore);
    }
    
    async recalc(standings: Standing[]) {
        standings.forEach(async standing => {
            const dto = new UpdateStandingDto();
            dto.points = standing.points;
            await this.standingService.update(standing.id, dto);   
        });
    }

    async RemoveStanding(playerId: number, songId: number) {
        //this.matchHub.OnMatchUpdate(activeMatch);
    }

    CountActivePlayersInRound(players: Player[], round: Round) {
        return players.flatMap((value) => round.disabledPlayerIds.includes(value.id)).length
    }
}