import { Injectable } from '@nestjs/common';
import { CreateStandingDto, UpdateStandingDto } from '../dtos/standing.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Standing } from '../entities/standing.entity'
import { Round } from '../entities/round.entity'
import { Score } from '../entities/score.entity'
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class StandingsService implements ICrudService<Standing, CreateStandingDto, UpdateStandingDto> {
    constructor(
        @InjectRepository(Standing)
        private standingRepo: Repository<Standing>,
        @InjectRepository(Score)
        private scoreRepo: Repository<Score>,
        @InjectRepository(Round)
        private roundRepo: Repository<Round>
    ) { }

    async create(dto: CreateStandingDto) {
        const newStanding = new Standing();

        const score = await this.scoreRepo.findOneBy({ id: dto.scoreId });

        if (!score) {
            throw Error(`Score with id ${dto.scoreId} not found. Insert standing failed`)
        }

        const round = await this.roundRepo.findOneBy({ id: dto.roundId });

        if (!round) {
            throw Error(`Round with id ${dto.roundId} not found. Insert standing failed`)
        }

        newStanding.score = score;
        newStanding.round = round;

        await this.standingRepo.insert(newStanding);

        return newStanding;
    }

    async findAll() {
        return await this.standingRepo.find();
    }

    async findOne(id: number) {
        return await this.standingRepo.findOneBy({ id });
    }

    async update(id: number, dto: UpdateStandingDto) {
        const score = await this.standingRepo.findOneBy({ id });

        if (!score) {
            throw Error(`Standing with id ${id} not found. Update standing failed`);
        }

        if (dto.scoreId) {
            const score = await this.scoreRepo.findOneBy({ id: dto.scoreId });

            if (!score) {
                throw Error(`Score with id ${dto.scoreId} not found. Update standing failed`)
            }

            dto.score = score;
            delete dto.scoreId;
        }

        if (dto.roundId) {
            const round = await this.roundRepo.findOneBy({ id: dto.roundId });
    
            if (!round) {
                throw Error(`Round with id ${dto.roundId} not found. Update standing failed`)
            }

            dto.round = round;
            delete dto.roundId;
        }

        await this.standingRepo.merge(score, dto);

        return score;
    }

    async remove(id: number) {
        await this.standingRepo.delete(id);
    }
}
