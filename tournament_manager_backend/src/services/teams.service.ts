import { Injectable } from '@nestjs/common';
import { CreateTeamDto, UpdateTeamDto } from '../dtos/team.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Team } from '../entities/team.entity'
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class TeamsService implements ICrudService<Team, CreateTeamDto, UpdateTeamDto> {
  constructor(
    @InjectRepository(Team)
    private repo: Repository<Team>,
  ) { }

  async create(dto: CreateTeamDto) {
    const newTeam = new Team();
    newTeam.name = dto.name;

    await this.repo.insert(newTeam);

    return newTeam;
  }

  async findAll() {
    return await this.repo.find();
  }

  async findOne(id: number) {
    return await this.repo.findOneBy({ id });
  }

  async update(id: number, dto: UpdateTeamDto) {
    const team = await this.repo.findOneBy({ id });

    if (!team) {
      throw Error(`Team with id ${id} not found. Update failed`)
    }

    await this.repo.merge(team, dto);

    return team;
  }

  async remove(id: number) {
    await this.repo.delete(id);
  }
}
