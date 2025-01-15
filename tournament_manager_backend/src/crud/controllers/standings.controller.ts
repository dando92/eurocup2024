import { Controller } from '@nestjs/common';
import { StandingsService } from '../services';
import { GenericController } from './generic.controller';
import { CreateStandingDto, UpdateStandingDto } from '../dtos';
import { Standing } from '../entities'

@Controller('standings')
export class StandingsController extends GenericController<Standing, CreateStandingDto, UpdateStandingDto, StandingsService> { }
