import { Controller } from '@nestjs/common';
import { DivisionsService } from '../services';
import { GenericController } from './generic.controller';

@Controller('divisions')
export class DivisionsController extends GenericController<DivisionsService> { }

