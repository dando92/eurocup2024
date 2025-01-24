import { Injectable, Inject } from "@nestjs/common";
import { PlayerService, SongService, StandingsService, ScoresService } from "src/crud/services";
import { LiveScore } from "../gateways/live.score.gateway"
import { CreateScoreDto, CreateStandingDto, UpdateStandingDto } from "src/crud/dtos";
import { TournamentCache } from "./tournament.cache";
import { Standing } from "src/crud/entities";
import { MatchGateway } from '../gateways/match.gateway';

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
        private readonly matchHub: MatchGateway
    ) { }

    async AddScore(score: LiveScore) {
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

        const actualScoreEntity = await this.scoresService.create(newScore)

        const activeMatch = this.tournamentCache.GetActiveMatch();
        const round = activeMatch.rounds.find(async round=> (await round.song).id === song.id);

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

        
        if(round.standings.length >= activeMatch.players.length) {
            const updatedStandings = this.recalc(round.standings);
            
            //TODO: is it necessary?
            await this.standingService.update(updatedStandings[0], updatedStandings[1]);
        }

        this.matchHub.OnMatchUpdate(activeMatch);
    }
    
    async recalc(standings: Standing[]) : Promise<([id: number, UpdateStandingDto])[]> {
        return null;
    }

    async RemoveStanding(playerId: number, songId: number) {
        //this.matchHub.OnMatchUpdate(activeMatch);
    }
}