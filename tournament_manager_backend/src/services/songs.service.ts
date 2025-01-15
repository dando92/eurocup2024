import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import
 { Repository } from 'typeorm';
import { Song } from '../entities/song.entity';
import { CreateSongDto, UpdateSongDto } from '../dtos/song.dto';
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class SongService implements ICrudService<Song, CreateSongDto, UpdateSongDto> {
  constructor(
    @InjectRepository(Song)

    private songRepository: Repository<Song>,
  ) {}

  async create(createSongDto: CreateSongDto): Promise<Song> {
    const newSong = this.songRepository.create(createSongDto);
    return this.songRepository.save(newSong);
  }

  async findAll(): Promise<Song[]> {
    return this.songRepository.find();
  }

  async findOne(id: number): Promise<Song> {
    const song = await this.songRepository.findOneBy({ id });
    if (!song) {
      throw new NotFoundException(`Song with ID ${id} not found`);
    }
    return song;
  }

  async update(id: number, updateSongDto: UpdateSongDto): Promise<Song> {
    const song = await this.songRepository.findOneBy({ id });
    if (!song) {
      throw new NotFoundException(`Song with ID ${id} not found`);
    }

    this.songRepository.merge(song, updateSongDto);
    return song;
  }

  async remove(id: number): Promise<void> {
    const song = await this.songRepository.findOneBy({ id });
    if (!song) {
      throw new NotFoundException(`Song with ID ${id} not found`);
    }
    await this.songRepository.delete(id);
  }
}
