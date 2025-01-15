import { Controller } from '@nestjs/common';
import { RoundsService } from '../services';
import { GenericController } from './generic.controller';
import { CreateRoundDto, UpdateRoundDto } from '../dtos';
import { Round } from '../entities'

@Controller('rounds')
export class RoundsController extends GenericController<Round, CreateRoundDto, UpdateRoundDto, RoundsService> { }
