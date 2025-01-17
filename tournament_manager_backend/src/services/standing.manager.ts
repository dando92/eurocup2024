import { Injectable, Inject } from "@nestjs/common";
import { PlayerService, SongService, StandingsService } from "src/crud/services";

@Injectable()
export class StandingManager {
    constructor(
        @Inject()
        private readonly standingService: StandingsService,
        @Inject()
        private readonly songService: SongService,
        @Inject()
        private readonly playerService: PlayerService
    ) { }
}