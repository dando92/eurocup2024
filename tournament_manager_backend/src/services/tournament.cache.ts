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
        this.activeMatch = null;
    }

    public async SetActiveMatch(matchId: number) {
        if (matchId != this.activeMatchId) {
            this.activeMatchId = matchId;
            
            if(this.activeMatchId != 0) {
                this.activeMatch = await this.matchService.findOne(matchId);
            } else {
                this.activeMatch = null;
            }
        }
    }

    public GetActiveMatch() : Match | null {
        return this.activeMatch;
    }
}
