import { Controller, Delete, Get, Param, Post } from '@nestjs/common';
import { DivisionsService, PhasesService } from '../services';
import { Match, Phase } from '../entities';
import { TournamentCache } from 'src/services/tournament.cache';

@Controller('tournament')
export class BackwardCompatibilityController {
    constructor(private readonly phaseService: PhasesService,
        private readonly divisionService: DivisionsService,
        private readonly tournamentCache: TournamentCache,
    ) { }

    @Get('expandphase/:id')
    async allMatchesFrom(@Param('id') id: number): Promise<Match[]> {
        return this.phaseService.findOne(id)
            .then(body => {
                return body.matches;
            });
    }

    @Get(':id/phases')
    getPhaseFromDivision(@Param('id') id: number): Promise<Phase[] | null> {
        return this.divisionService.findOne(id)
            .then(body => {
                return body.phases
            }); 
    }

    @Get('activeMatch')
    getActiveMatch(): Match | null {
        return this.tournamentCache.GetActiveMatch();
    }

    @Get('possibleScoringSystem')
    getScoringSystem(@Param('id') id: number): string[] | null {
        return ["Eurocup"];
    }

    @Post(':playerId/removeFromTeam')
    removeFromTeam(@Param('playerId') playerId: number): string[] | null {
        return ["Eurocup"];
    }

    @Post('setactivematch')
    setActiveMatch(): string[] | null {
        return ["Eurocup"];
    }

    @Post('addsongtomatch')
    addSongToMatch(): string[] | null {
        return ["Eurocup"];
    }

    @Post('editmatchsong')
    editMatchSong(): string[] | null {
        return ["Eurocup"];
    }

    @Post('addstanding')
    addStanding(): string[] | null {
        return ["Eurocup"];
    }
    @Delete('deletestanding/:playerId/:songId')
    deleteStanding(@Param('playerId') playerId: number, @Param('songId') songId: number): string[] | null {
        return ["Eurocup"];
    }
}
