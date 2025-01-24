import { Controller, Delete, Get, Param, Post, Body } from '@nestjs/common';
import { DivisionsService, MatchesService, PhasesService, PlayerService } from './crud/services';
import { Match, Phase, Player } from './crud/entities';
import { TournamentCache } from 'src/services/tournament.cache';
import { CreateMatchDto, UpdatePlayerDto } from './crud/dtos';
import { MatchManager } from './services/match.manager';
import { StandingManager } from './services/standing.manager';
import { LiveScore } from './gateways/live.score.gateway';

export class SetActiveMatchDTO {
    matchId: number;
}

export class PostAddMatch {
    divisionId: number;
    phaseId: number;
    matchName: string;
    group: string;
    subtitle: string;
    isManualMatch: boolean;
    multiplier: number;
    notes: string;
    levels: string;
    songIds: number[];
    scoringSystem: string;
    playerIds: number[];
}


export class PostAddSongToMatch {
    matchId: number;
    group: string;
    level: string;
    songId: number;
    divisionId: number;
}

export class PostEditSongToMatch extends PostAddSongToMatch {
    editSongId: number;
}


export class PostStanding {
    id: number;
    songId: number;
    playerId: number;
    percentage: number;
    roundId: number;
    score: number;
    isFailed: boolean;
}

@Controller('tournament')
export class BackwardCompatibilityController {
    constructor(private readonly phaseService: PhasesService,
        private readonly divisionService: DivisionsService,
        private readonly playerService: PlayerService,
        private readonly tournamentCache: TournamentCache,
        private readonly matchManager: MatchManager,
        private readonly matchService: MatchesService,
        private readonly standingManager: StandingManager,
    ) { }

    @Get('expandphase/:id')
    async allMatchesFrom(@Param('id') id: number): Promise<Match[]> {
        return (await this.phaseService.findOne(id)).matches;
    }

    @Get(':id/phases')
    async getPhaseFromDivision(@Param('id') id: number): Promise<Phase[] | null> {
        return await (await this.divisionService.findOne(id)).phases;
    }

    @Get('activeMatch')
    getActiveMatch(): Match | null {
        return this.tournamentCache.GetActiveMatch();
    }

    @Get('possibleScoringSystem')
    getScoringSystem(@Param('id') id: number): string[] | null {
        return ["Eurocup 2025"];
    }

    @Post(':playerId/removeFromTeam')
    async removeFromTeam(@Param('playerId') playerId: number): Promise<Player | null> {
        const dto = new UpdatePlayerDto();
        dto.teamId = null;
        
        return await this.playerService.update(playerId, dto);
    }

    @Post('setActiveMatch')
    async setActiveMatch(@Body() dto: SetActiveMatchDTO): Promise<Match | null> {
        await this.tournamentCache.SetActiveMatch(dto.matchId);
        
        return this.getActiveMatch();
    }

    @Post('addSongToMatch')
    async addSongToMatch(@Body() dto: PostAddSongToMatch): Promise<Match | null> {
        if(dto.songId) {
            await this.matchManager.AddSongsToMatchById(dto.matchId, [dto.songId])
        }
        else if(dto.level) {
            await this.matchManager.AddRandomSongsToMatchById(dto.matchId, dto.divisionId, dto.group, dto.level);
        }
        
        return this.getActiveMatch();
    }
    
    
    @Post('addMatch')
    async addMatch(@Body() dto: PostAddMatch): Promise<Match> {
        const newMatchDto = new CreateMatchDto();
        newMatchDto.name = dto.matchName;
        newMatchDto.notes = dto.notes;
        newMatchDto.phaseId = dto.phaseId;
        newMatchDto.playerIds = dto.playerIds;
        newMatchDto.subtitle = dto.subtitle;

        const match = await this.matchService.create(newMatchDto);

        if(dto.songIds) {
            await this.matchManager.AddSongsToMatch(match, dto.songIds)
        }
        else if(dto.levels) {
            await this.matchManager.AddRandomSongsToMatch(match, dto.divisionId, dto.group, dto.levels);
        }

        return match;
    }
    
    @Post('editMatchSong')
    async editMatchSong(@Body() dto: PostEditSongToMatch): Promise<Match | null> {
        this.matchManager.RemoveSongFromMatchById(dto.matchId, dto.songId);
        return await this.addSongToMatch(dto);
    }

    @Post('addstanding')
    async addStanding(@Body() dto: PostStanding): Promise<Match | null> {
        //TODO:
        // const liveScore = new LiveScore();
        // liveScore = dto.
        // this.standingManager.AddScore()
        return this.getActiveMatch();
    }

    @Delete('deletestanding/:playerId/:songId')
    async deleteStanding(@Param('playerId') playerId: number, @Param('songId') songId: number): Promise<Match | null> {
        this.standingManager.RemoveStanding(playerId, songId);
        return this.getActiveMatch();
    }
}
