import { Controller } from '@nestjs/common';
import { StandingsService } from '../services';
import { GenericController } from './generic.controller';

@Controller('standings')
export class StandingsController extends GenericController<StandingsService> { }
