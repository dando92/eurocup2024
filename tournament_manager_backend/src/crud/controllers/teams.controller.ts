import { Controller } from '@nestjs/common';
import { TeamsService } from '../services';
import { GenericController } from './generic.controller';
import { CreateTeamDto, UpdateTeamDto } from '../dtos';
import { Team } from '../entities'

@Controller('teams')
export class TeamsController extends GenericController<Team, CreateTeamDto, UpdateTeamDto, TeamsService> { }