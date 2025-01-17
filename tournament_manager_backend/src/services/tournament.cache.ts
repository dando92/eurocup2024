import { Injectable, Inject } from '@nestjs/common';
import { MatchesService } from '../crud/services';
import { Match } from '../crud/entities/match.entity';

@Injectable()
export class TournamentCache {
    activeMatchId: number;
    activeMatch: Match;
    constructor(
        @Inject()
        private readonly matchService: MatchesService
    ) {
        this.activeMatchId = 0;
    }

    public async SetActiveMatch(matchId: number) {
        if (matchId != this.activeMatchId) {
            this.activeMatchId = matchId;
            this.activeMatch = await this.matchService.findOne(matchId);
        }
    }

    public GetActiveMatch() {
        return this.activeMatch;
    }
}
