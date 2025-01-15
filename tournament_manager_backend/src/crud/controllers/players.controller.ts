import { Controller } from '@nestjs/common';
import { GenericController } from './generic.controller';
import { PlayerService } from '../services';
import { CreatePlayerDto, UpdatePlayerDto } from '../dtos';
import { Player } from '../entities'

@Controller('players')
export class PlayersController extends GenericController<Player, CreatePlayerDto, UpdatePlayerDto, PlayerService> { }