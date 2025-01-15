import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Tournament } from '../entities/tournament.entity';
import { CreateTournamentDto, UpdateTournamentDto } from '../dtos/tournament.dto';
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class TournamentsService implements ICrudService<Tournament, CreateTournamentDto, UpdateTournamentDto> {
  constructor(
    @InjectRepository(Tournament)
    private readonly tournamentsRepository: Repository<Tournament>
  ) {}

  async create(dto: CreateTournamentDto){
    const tournament = new Tournament();
    
    tournament.name = dto.name;

    await this.tournamentsRepository.insert(tournament);

    return tournament;
  }

  async findAll(): Promise<Tournament[]> {
    return this.tournamentsRepository.find();
  }

  async findOne(id: number): Promise<Tournament> {
    return this.tournamentsRepository.findOneBy({ id });
  }

  async update(id: number, dto: UpdateTournamentDto){
    const tournament = await this.findOne(id);

    if (!tournament) {
        throw new Error(`Tournament with ID ${id} not found`);
    }

    await this.tournamentsRepository.merge(tournament, dto);

    return tournament;
  }

  async remove(id: number){
    await this.tournamentsRepository.delete(id);
  }
}
