
import { Controller } from '@nestjs/common';
import { PhasesService } from '../services';
import { GenericController } from './generic.controller';
import { Phase } from '../entities';
import { CreatePhaseDto, UpdatePhaseDto } from '../dtos';

@Controller('phases')
export class PhasesController extends GenericController<Phase, CreatePhaseDto, UpdatePhaseDto, PhasesService> { }