import { Injectable } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { Match } from "src/crud/entities";
import { MatchAssignment } from "src/crud/entities/match_assignment.entity";
import { Setup } from "src/crud/entities/setup.entity";
import { Repository } from "typeorm";

@Injectable()
export class PadRoller {
    constructor(
        @InjectRepository(MatchAssignment)
        private matchAssignmentRepository: Repository<MatchAssignment>) { }

    async RollPadForMatch(match:Match) {
        const rounds = match.rounds;
        const players = match.players;
        const setups: Setup[] = null;

        setups.sort((a, b) => a.position - b.position);

        for (let i = 0; i < rounds.length; i++) {
            const matchAssignment = new MatchAssignment();
            const matchAssignments:MatchAssignment[] = [];

            for (let j = 0; j < players.length; j++) {
                matchAssignment.player = players[j];
                matchAssignment.setup = setups[j];
                matchAssignment.round = rounds[i];
                
                this.matchAssignmentRepository.insert(matchAssignment);
                matchAssignments.push(matchAssignment);
            }
            setups.push(setups.shift());

            match.rounds[i].matchAssignments = matchAssignments;
        }

        return match;
    }
}