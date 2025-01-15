import { Controller } from '@nestjs/common';
import { GenericController } from './generic.controller';
import { PlayerService } from '../services';

@Controller('players')
export class PlayersController extends GenericController<PlayerService> { }