import { Controller } from '@nestjs/common';
import { TeamsService } from '../services';
import { GenericController } from './generic.controller';

@Controller('teams')
export class TeamsController extends GenericController<TeamsService> { }