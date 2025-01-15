import { Controller } from '@nestjs/common';
import { TournamentsService } from '../services';
import { GenericController } from './generic.controller';

@Controller('tournaments')
export class TournamentsController extends GenericController<TournamentsService> { }