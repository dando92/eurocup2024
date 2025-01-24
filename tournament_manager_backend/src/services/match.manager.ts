import { Injectable, Inject } from '@nestjs/common';
import { MatchesService, RoundsService } from '../crud/services';
import { CreateRoundDto } from '../crud/dtos';
import { Match } from '../crud/entities/match.entity';
import { SongExtractor } from './song.extractor';
import { MatchGateway } from '../gateways/match.gateway';

@Injectable()
export class MatchManager {
    constructor(
        @Inject()
        private readonly matchService: MatchesService,
        @Inject()
        private readonly roundService: RoundsService,
        @Inject()
        private readonly songExtractor: SongExtractor,
        @Inject()
        private readonly matchHub: MatchGateway
    ) { }
    
    public async AddSongsToMatchById(matchId: number, songIds: number[]): Promise<void> {
        const match = await this.matchService.findOne(matchId);

        if (!match) {
            return;
        }

        await this.AddSongsToMatch(match, songIds);
    }

    public async AddRandomSongsToMatchById(matchId: number, divisionId: number, group: string, levels: string): Promise<void> {
        const match = await this.matchService.findOne(matchId);

        if (!match) {
            return;
        }

        await this.AddRandomSongsToMatch(match, divisionId, group, levels);
    }

    public async RemoveSongFromMatchById(matchId: number, songId: number): Promise<void> {
        const match = await this.matchService.findOne(matchId);

        if (!match) {
            return;
        }

        await this.RemoveSongFromMatch(match, songId);
    }

    private async RemoveSongFromMatch(match: Match, songId: number): Promise<void> {
        const round = match.rounds.find(round => round.song.id == songId);

        if (!round) {
            return;
        }

        //TODO: how to request "are you sure???" when standings are filled
        if(round.standings.length > 0){
            return;
        }

        await this.roundService.remove(round.id);
        
        this.matchHub.OnMatchUpdate(match);
    }

    public async AddRandomSongsToMatch(match: Match, divisionId: number, group: string, levels: string): Promise<void> {
        const songIds = await this.songExtractor.RollSongs(divisionId, group, levels);

        await this.AddSongsToMatch(match, songIds);
    }

    public async AddSongsToMatch(match: Match, songIds: number[]): Promise<void> {
        for (const songId of songIds) {
            await this.AddSongToMatch(match, songId);
        }
        
        this.matchHub.OnMatchUpdate(match);
    }

    private async AddSongToMatch(match: Match, songId: number): Promise<void> {
        await this.roundService.create(this.GetRoundDto(match, songId));
    }

    private GetRoundDto(match: Match, songId: number): CreateRoundDto {
        const dto =  new CreateRoundDto();
        dto.matchId = match.id;
        dto.songId = songId;
        return dto;
    }
}