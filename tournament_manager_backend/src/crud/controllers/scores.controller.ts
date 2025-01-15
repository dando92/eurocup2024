import { Controller } from '@nestjs/common';
import { ScoresService } from '../services';
import { GenericController } from './generic.controller';
import { CreateScoreDto, UpdateScoreDto } from '../dtos';
import { Score } from '../entities'

@Controller('scores')
export class ScoresController extends GenericController<Score, CreateScoreDto, UpdateScoreDto, ScoresService> { }
