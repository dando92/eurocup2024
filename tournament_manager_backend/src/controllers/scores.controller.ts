import { Controller } from '@nestjs/common';
import { ScoresService } from '../services';
import { GenericController } from './generic.controller';

@Controller('scores')
export class ScoresController extends GenericController<ScoresService> { }
