import { Controller } from '@nestjs/common';
import { MatchesService } from '../services';
import { GenericController } from './generic.controller';
import { CreateMatchDto, UpdateMatchDto } from '../dtos';
import { Phase, Player, Match } from '../entities';

@Controller('matches')
export class MatchesController extends GenericController<Match, CreateMatchDto, UpdateMatchDto, MatchesService> { }
