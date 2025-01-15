import { Controller } from '@nestjs/common';
import { RoundsService } from '../services';
import { GenericController } from './generic.controller';

@Controller('rounds')
export class RoundsController extends GenericController<RoundsService> { }
