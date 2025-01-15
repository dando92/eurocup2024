
import { Controller } from '@nestjs/common';
import { PhasesService } from '../services';
import { GenericController } from './generic.controller';

@Controller('phases')
export class PhasesController extends GenericController<PhasesService> { }