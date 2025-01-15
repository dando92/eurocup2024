import { Controller } from '@nestjs/common';
import { MatchesService } from '../services';
import { GenericController } from './generic.controller';

@Controller('matches')
export class MatchesController extends GenericController<MatchesService> { }
