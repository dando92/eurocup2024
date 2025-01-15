
import { Controller } from '@nestjs/common';
import { SongService } from '../services';
import { GenericController } from './generic.controller';
import { Song } from '../entities';
import { CreateSongDto, UpdateSongDto } from '../dtos';

@Controller('songs')
export class SongsController extends GenericController<Song, CreateSongDto, UpdateSongDto, SongService> { }