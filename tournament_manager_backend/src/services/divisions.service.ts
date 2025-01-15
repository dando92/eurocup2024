import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Division, Tournament } from '../entities';
import { CreateDivisionDto, UpdateDivisionDto } from '../dtos';
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class DivisionsService implements ICrudService<Division, CreateDivisionDto, UpdateDivisionDto> {
    constructor(
        @InjectRepository(Division)
        private divisionRepository: Repository<Division>,
        @InjectRepository(Tournament)
        private tournamentRepository: Repository<Tournament>,
    ) { }

    async create(dto: CreateDivisionDto) {
        const tournament = await this.tournamentRepository.findOneBy({ id: dto.tournamentId });

        if (!tournament) {
            throw new NotFoundException(`Tournament with ID ${dto.tournamentId} not found`);
        }

        const division = new Division();

        division.name = dto.name;
        division.tournament = tournament;

        await this.divisionRepository.insert(division);

        return division;
    }

    async findAll() {
        return await this.divisionRepository.find();
    }

    async findOne(id: number) {
        return await this.divisionRepository.findOneBy({ id });
    }

    async update(id: number, dto: UpdateDivisionDto) {
        const division = await this.divisionRepository.findOneBy({ id });

        if (!division) {
            throw new NotFoundException(`Division with ID ${id} not found`);
        }

        // Check and update tournament if provided
        if (dto.tournamentId) {
            const tournament = await this.tournamentRepository.findOneBy({ id: dto.tournamentId });
            if (!tournament) {
                throw new NotFoundException(`Tournament with ID ${dto.tournamentId} not found`);
            }
            division.tournament = tournament;
            delete dto.tournamentId;
        }

        this.divisionRepository.merge(division, dto);

        return await this.divisionRepository.save(division);
    }

    async remove(id: number) {
        await this.divisionRepository.delete(id);
    }
}
