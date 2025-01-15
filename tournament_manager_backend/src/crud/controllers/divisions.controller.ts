import { Controller } from '@nestjs/common';
import { DivisionsService } from '../services';
import { GenericController } from './generic.controller';
import { Division } from '../entities';
import { CreateDivisionDto, UpdateDivisionDto } from '../dtos';

@Controller('divisions')
export class DivisionsController extends GenericController<Division, CreateDivisionDto, UpdateDivisionDto, DivisionsService> { }

