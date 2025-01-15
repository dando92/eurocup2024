import { Injectable } from '@nestjs/common';
import { CreateScoreDto, UpdateScoreDto } from '../dtos/score.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Score } from '../entities/score.entity'
import { Song } from '../entities/song.entity'
import { Player } from '../entities/player.entity'
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class ScoresService implements ICrudService<Score, CreateScoreDto, UpdateScoreDto> {
  constructor(
    @InjectRepository(Score)
    private scoreRepository: Repository<Score>,
    @InjectRepository(Song)
    private songRepository: Repository<Song>,
    @InjectRepository(Player)
    private playerRepository: Repository<Player>,
  ) { }

  async create(createScoreDto: CreateScoreDto) {
    const song = await this.songRepository.findOneBy({ id: createScoreDto.songId });
    const player = await this.playerRepository.findOneBy({ id: createScoreDto.playerId });

    if (!song || !player) {
      throw new Error('Song or Player not found');
    }

    const newScore = new Score();

    newScore.percentage = createScoreDto.percentage;
    newScore.score = createScoreDto.score;
    newScore.isFailed = createScoreDto.isFailed;
    newScore.song = song;
    newScore.player = player;

    await this.scoreRepository.insert(newScore);

    return newScore;
  }

  async findAll() {
    return await this.scoreRepository.find();
  }

  async findOne(id: number) {
    return await this.scoreRepository.findOneBy({ id });
  }

  async update(id: number, dto: UpdateScoreDto) {
    const score = await this.scoreRepository.findOneBy({ id });

    if (!score) {
      throw new Error(`Score with id ${id} not found`)
    }

    //Check if the song or the player are updated
    if (dto.songId) {
      const song = await this.songRepository.findOneBy({ id: dto.songId });
      if (!song) {
        throw new Error(`Song with id ${dto.songId} not found`)
      }
      dto.song = song
      delete dto.songId
    }

    if (dto.playerId) {
      const player = await this.playerRepository.findOneBy({ id: dto.playerId });
      if (!player) {
        throw new Error(`Player with id ${dto.playerId} not found`)
      }
      dto.player = player
      delete dto.playerId
    }

    await this.scoreRepository.merge(score, dto);

    return score;
  }

  async remove(id: number) {
    await this.scoreRepository.delete(id);
  }
}
