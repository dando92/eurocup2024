import { Controller } from '@nestjs/common';
import { TournamentsService } from '../services';
import { GenericController } from './generic.controller';
import { Tournament } from '../entities';
import { CreateTournamentDto, UpdateTournamentDto } from '../dtos';

@Controller('tournaments')
export class TournamentsController extends GenericController<Tournament, CreateTournamentDto, UpdateTournamentDto, TournamentsService> { }