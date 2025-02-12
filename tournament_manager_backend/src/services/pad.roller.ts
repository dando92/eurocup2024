import { Inject, Injectable } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { CreateMatchAssignmentDto } from "src/crud/dtos";
import { Match } from "src/crud/entities";
import { MatchAssignment } from "src/crud/entities/match_assignment.entity";
import { Setup } from "src/crud/entities/setup.entity";
import { MatchAssignmentService } from "src/crud/services";
import { Repository } from "typeorm";

@Injectable()
export class PadRoller {
    constructor(
        @Inject()
        private matchAssignmentService: MatchAssignmentService) { }

    async AutoRollPads(match: Match) {
        const rounds = match.rounds;
        const players = match.players;
        const setups: Setup[] = null;

        setups.sort((a, b) => a.position - b.position);

        for (let i = 0; i < rounds.length; i++) {
            const matchAssignment = new CreateMatchAssignmentDto();
            const matchAssignments: MatchAssignment[] = [];

            for (let j = 0; j < players.length; j++) {
                matchAssignment.playerId = players[j].id;
                matchAssignment.setupId = setups[j].id;
                matchAssignment.roundId = rounds[i].id;

                matchAssignments.push(await this.matchAssignmentService.create(matchAssignment));
            }

            setups.push(setups.shift());

            match.rounds[i].matchAssignments = matchAssignments;
        }

        return match;
    }
}