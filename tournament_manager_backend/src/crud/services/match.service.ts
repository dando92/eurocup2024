import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { CreateMatchDto, UpdateMatchDto } from '../dtos';
import { Phase, Player, Match } from '../entities';

@Injectable()
export class MatchesService{
    constructor(
        @InjectRepository(Match)
        private matchRepository: Repository<Match>,
        @InjectRepository(Phase)
        private phaseRepository: Repository<Phase>,
        @InjectRepository(Player)
        private playerRepository: Repository<Player>,
    ) { }

    async create(dto: CreateMatchDto) {
        const match = new Match();

        const phase = await this.phaseRepository.findOneBy({ id: dto.phaseId });

        if (!phase) {
            throw new NotFoundException(`Phase with ID ${dto.phaseId} not found`);
        }
        match.phase = phase;

        match.players = [];

        for (const playerId of dto.playerIds) {
            const player = await this.playerRepository.findOneBy({ id: playerId });

            if (!player) {
                throw new NotFoundException(`Player with ID ${playerId} not found`);
            }
            match.players.push(player);
        }

        match.name = dto.name;
        match.notes = dto.notes;
        match.subtitle = dto.subtitle;

        await this.matchRepository.insert(match);

        return match;
    }

    async findAll() {
        return await this.matchRepository.find();
    }

    async findOne(id: number) {
        return await this.matchRepository.findOneBy({ id });
    }

    async update(id: number, dto: UpdateMatchDto) {
        const match = await this.matchRepository.findOneBy({ id });

        if (!match) {
            throw new Error(`Match with ID ${id} not found`);
        }

        if (dto.phaseId) {
            const phase = await this.phaseRepository.findOneBy({ id: dto.phaseId });
            if (!phase) {
                throw new NotFoundException(`Phase with ID ${dto.phaseId} not found`);
            }
            match.phase = phase;
            delete dto.phaseId
        }

        if (dto.playerIds) {
            const players = [];
            for (const playerId of dto.playerIds) {
                const player = await this.playerRepository.findOneBy({ id: playerId });

                if (!player) {
                    throw new NotFoundException(`Player with ID ${playerId} not found`);
                }
                players.push(player);
            }

            dto.players = players
            delete dto.playerIds
        }

        this.matchRepository.merge(match, dto);

        return match;
    }

    async remove(id: number) {
        await this.matchRepository.delete(id);
    }
}
