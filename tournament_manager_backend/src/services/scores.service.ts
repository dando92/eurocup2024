import { Injectable } from '@nestjs/common';
import { CreateScoreDto, UpdateScoreDto } from '../dtos/score.dto';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Score } from '../entities/score.entity'
import { Song } from '../entities/song.entity'
import { Player } from '../entities/player.entity'

@Injectable()
export class ScoresService {
  constructor(
    @InjectRepository(Score)
    private repo: Repository<Score>,
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

    await this.repo.insert(newScore);

    return newScore;
  }

  async findAll() {
    return await this.repo.find();
  }

  async findOne(id: number) {
    return await this.repo.findOneBy({ id });
  }

  async update(id: number, updateScoreDto: UpdateScoreDto) {
    const score = await this.repo.findOneBy({ id });

    if (!score) {
      throw new Error(`Score with id ${id} not found`)
    }

    //Check if the song or the player are updated
    if (updateScoreDto.songId) {
      const song = await this.songRepository.findOneBy({ id: updateScoreDto.songId });
      if (!song) {
        throw new Error(`Song with id ${updateScoreDto.songId} not found`)
      }
    }

    if (updateScoreDto.playerId) {
      const player = await this.playerRepository.findOneBy({ id: updateScoreDto.playerId });
      if (!player) {
        throw new Error(`Player with id ${updateScoreDto.playerId} not found`)
      }
    }

    await this.repo.update({ id: id }, updateScoreDto);

    return score;
  }

  async remove(id: number) {
    await this.repo.delete(id);
  }
}
