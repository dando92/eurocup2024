import { Injectable } from '@nestjs/common';
import { CreatePlayerDto, UpdatePlayerDto } from '../dtos/player.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Player } from '../entities/player.entity'
import { Team } from '../entities/team.entity'

@Injectable()
export class PlayerService {
  constructor(
    @InjectRepository(Player)
    private playersRepo: Repository<Player>,
    @InjectRepository(Team)
    private teamsRepo: Repository<Team>
  ) { }

  async create(dto: CreatePlayerDto) {
    if (!dto.name) {
      throw Error(`A name shall be provided. Insert player failed`)
    }

    const newPlayer = new Player();
    newPlayer.name = dto.name;
    
    if (dto.teamId) {
      const team = await this.teamsRepo.findOneBy({ id: dto.teamId });
      if (!team) {
        throw Error(`Team with id ${dto.teamId} not found. Insert player failed`)
      }
      newPlayer.team = team;
    }

    await this.playersRepo.insert(newPlayer);

    return newPlayer;
  }

  async findAll() {
    return await this.playersRepo.find();
  }

  async findOne(id: number) {
    return await this.playersRepo.findOneBy({ id });
  }

  async update(id: number, dto: UpdatePlayerDto) {
    const player = await this.playersRepo.findOneBy({ id });

    if (!player) {
      throw Error(`Player with id ${id} not found. Update player failed`)
    }

    const updateDto = undefined;

    if (dto.name) {
      updateDto.name = dto.name;
    }

    if (dto.teamId) {
      const team = await this.teamsRepo.findOneBy({ id: dto.teamId });
      if (!team) {
        throw Error(`Team with id ${dto.teamId} not found. Update player failed`)
      }

      updateDto.team = team;
    }

    await this.playersRepo.update({ id: id }, updateDto);

    return player;
  }

  async remove(id: number) {
    await this.playersRepo.delete(id);
  }
}
