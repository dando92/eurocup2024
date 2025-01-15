
import { Controller } from '@nestjs/common';
import { SongService } from '../services';
import { GenericController } from './generic.controller';

@Controller('songs')
export class SongsController extends GenericController<SongService> { }