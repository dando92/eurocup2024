import { Controller, Delete, Get, Param, Post, Body } from '@nestjs/common';
import { DivisionsService, MatchesService, PhasesService, PlayerService, RoundsService, SongService } from './crud/services';
import { Match, Phase, Player, Song, Standing } from './crud/entities';
import { TournamentCache } from 'src/services/tournament.cache';
import { CreateMatchDto, CreateScoreDto, UpdatePlayerDto, UpdateRoundDto, CreateSongDto, CreatePlayerDto } from './crud/dtos';
import { MatchManager } from './services/match.manager';
import { StandingManager } from './services/standing.manager';
import { LiveScore } from './gateways/live.score.gateway';
import { Transform } from 'class-transformer';
import { ScoringSystemProvider } from './services/IScoringSystem';

export class PostBatchSongRequest{
    songs: CreateSongDto[]
}
export class PostBatchPlayerRequest{
    players: CreatePlayerDto[]
}

export class RoundDTO {
    id: number;
    song: Song;
    standings: Standing[]
}

export class MatchDto {
    id: number;
    phaseId: number;
    name: string;
    subtitle: string;
    notes: string;
    multiplier: number;
    isManualMatch: boolean;

    @Transform(({ value }) => value ?? [])
    songs: Song[];

    @Transform(({ value }) => value ?? [])
    players: Player[];

    @Transform(({ value }) => value ?? [])
    rounds: RoundDTO[];
}

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
        private readonly songService: SongService,
        private readonly tournamentCache: TournamentCache,
        private readonly matchManager: MatchManager,
        private readonly matchService: MatchesService,
        private readonly standingManager: StandingManager,
        private readonly roundService: RoundsService,
        private readonly scoringSystemProvider: ScoringSystemProvider
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
    async getActiveMatch(): Promise<Match | null> {
        return await this.tournamentCache.GetActiveMatch();
    }

    @Get('possibleScoringSystem')
    getScoringSystem(@Param('id') id: number): string[] | null {
        return this.scoringSystemProvider.getAll();
    }

    @Post('AddBatchSongs')
    async addBatchSongs(@Body() dto: PostBatchSongRequest) {
        dto.songs.forEach(async song => {
            await this.songService.create(song);
        });
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
    async addSongToMatch(@Body() dto: PostAddSongToMatch): Promise<MatchDto | null> {
        const match = await this.matchService.findOne(dto.matchId);

        if (dto.songId) {
            await this.matchManager.AddSongsToMatch(match, [dto.songId])
        }
        else if (dto.level) {
            await this.matchManager.AddRandomSongsToMatch(match, dto.divisionId, dto.group, dto.level);
        }

        return await this.convert(match);
    }


    @Post('addMatch')
    async addMatch(@Body() dto: PostAddMatch): Promise<MatchDto> {
        const newMatchDto = new CreateMatchDto();
        newMatchDto.name = dto.matchName;
        newMatchDto.notes = dto.notes;
        newMatchDto.phaseId = dto.phaseId;
        newMatchDto.playerIds = dto.playerIds;
        newMatchDto.subtitle = dto.subtitle;
        newMatchDto.scoringSystem = dto.scoringSystem;
        
        const match = await this.matchService.create(newMatchDto);

        if (dto.songIds) {
            await this.matchManager.AddSongsToMatch(match, dto.songIds)
        }
        else if (dto.levels) {
            await this.matchManager.AddRandomSongsToMatch(match, dto.divisionId, dto.group, dto.levels);
        }

        return await Promise.resolve(this.convert(match));
    }

    @Post('editMatchSong')
    async editMatchSong(@Body() dto: PostEditSongToMatch): Promise<MatchDto | null> {
        this.matchManager.RemoveSongFromMatchById(dto.matchId, dto.songId);
        return await this.addSongToMatch(dto);
    }

    @Post('addstanding')
    async addStanding(@Body() dto: PostStanding): Promise<MatchDto | null> {
        if(dto.isFailed && dto.percentage == -1) {
            const match = await this.getActiveMatch();
            const round = match.rounds.find(round => round.song.id == dto.songId);
            
            if(round.disabledPlayerIds == null)
                round.disabledPlayerIds = []

            round.disabledPlayerIds.push(dto.playerId);
            
            const roundDTO = new UpdateRoundDto();
            roundDTO.disabledPlayerIds = round.disabledPlayerIds;

            await this.roundService.update(round.id, roundDTO);
        }
        
        const score = new CreateScoreDto();
        
        score.isFailed = dto.isFailed;
        score.percentage = dto.percentage;
        score.playerId = dto.playerId;
        score.songId = dto.songId;

        await this.standingManager.AddScore(score)

        return await this.convert(await this.getActiveMatch());
    }

    @Delete('deletestanding/:playerId/:songId')
    async deleteStanding(@Param('playerId') playerId: number, @Param('songId') songId: number): Promise<MatchDto | null> {
        this.standingManager.RemoveStanding(playerId, songId);
        return await this.convert(await this.getActiveMatch());
    }
    
    @Post(':playerId/assignToTeam/:teamId')
    async assignToTeam(@Param('playerId') playerId: number, @Param('teamId') teamId: number) {
        const dto = new UpdatePlayerDto();
        dto.teamId = teamId;
        return await this.playerService.update(playerId, dto);
    }

    async convert(match: Match) {
        const dto = new MatchDto();
        dto.id = match.id;
        dto.isManualMatch = match.isManualMatch;
        dto.multiplier = match.multiplier;
        dto.name = match.name;
        dto.notes = match.notes;
        dto.phaseId = (await match.phase).id;
        dto.players = match.players;
        dto.songs = match.rounds.flatMap(r => r.song);
        dto.subtitle = match.subtitle;
        dto.rounds = [];
        match.rounds.forEach(element => {
            const roundDto = new RoundDTO();
            roundDto.id = element.id;
            roundDto.song = element.song;
            if(element.standings == null) {
                roundDto.standings = []
            } else {
                roundDto.standings = element.standings;
            }
            dto.rounds.push(roundDto);
        });
        return dto;
    }
}
