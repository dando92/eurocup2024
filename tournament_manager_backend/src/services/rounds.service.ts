import { Injectable } from '@nestjs/common';
import { CreateRoundDto, UpdateRoundDto } from '../dtos/round.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Round } from '../entities/round.entity'
import { Song } from '../entities/song.entity'
import { Match } from '../entities/match.entity'
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class RoundsService implements ICrudService<Round, CreateRoundDto, UpdateRoundDto> {
    constructor(
        @InjectRepository(Round)
        private playersRepo: Repository<Round>,
        @InjectRepository(Match)
        private matchRepo: Repository<Match>,
        @InjectRepository(Song)
        private songRepo: Repository<Song>,
    ) { }

    async create(dto: CreateRoundDto) {
        const newRound = new Round();

        if (dto.matchId) {
            const match = await this.matchRepo.findOneBy({ id: dto.matchId });

            if (!match) {
                throw Error(`Match with id ${dto.matchId} not found. Insert round failed`)
            }

            newRound.match = match;
        }

        if (dto.songId) {
            const song = await this.songRepo.findOneBy({ id: dto.songId });

            if (!song) {
                throw Error(`Song with id ${dto.songId} not found. Insert round failed`)
            }

            newRound.song = song;
        }

        await this.playersRepo.insert(newRound);

        return newRound;
    }

    async findAll() {
        return await this.playersRepo.find();
    }

    async findOne(id: number) {
        return await this.playersRepo.findOneBy({ id });
    }

    async update(id: number, dto: UpdateRoundDto) {
        const round = await this.playersRepo.findOneBy({ id });

        if (!round) {
            throw Error(`Round with id ${id} not found. Update round failed`);
        }

        if (dto.matchId) {
            const match = await this.matchRepo.findOneBy({ id: dto.matchId });
            if (!match) {
                throw Error(`Match with id ${dto.matchId} not found. Update round failed`);
            }
            dto.match = match;
            delete dto.matchId;
        }

        if (dto.songId) {
            const song = await this.songRepo.findOneBy({ id: dto.songId });
            if (!song) {
                throw Error(`Song with id ${dto.matchId} not found. Update round failed`);
            }
            dto.song = song;
            delete dto.songId;
        }

        await this.playersRepo.merge(round, dto);

        return round;
    }

    async remove(id: number) {
        await this.playersRepo.delete(id);
    }
}
