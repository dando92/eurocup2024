import { Injectable } from '@nestjs/common';
import { CreatePlayerDto, UpdatePlayerDto } from '../dtos/player.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Player } from '../entities/player.entity'
import { Team } from '../entities/team.entity'
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class PlayerService implements ICrudService<Player, CreatePlayerDto, UpdatePlayerDto> {
  constructor(
    @InjectRepository(Player)
    private playersRepo: Repository<Player>,
    @InjectRepository(Team)
    private teamsRepo: Repository<Team>
  ) { }

  async create(dto: CreatePlayerDto) {
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
      throw Error(`Player with id ${id} not found. Update player failed`);
    }

    if (dto.teamId) {
      const team = await this.teamsRepo.findOneBy({ id: dto.teamId });
      if (!team) {
        throw Error(`Team with id ${dto.teamId} not found. Update player failed`);
      }
      dto.team = team;
      delete dto.teamId;
    }

    await this.playersRepo.merge(player, dto);

    return player;
  }

  async remove(id: number) {
    await this.playersRepo.delete(id);
  }
}
