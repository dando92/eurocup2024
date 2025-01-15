import {
  IsNotEmpty,
  IsNumber,
  IsArray,
  IsOptional,
  IsString,
} from 'class-validator';
import { Player, Phase } from '../entities';

export class CreateMatchDto {
  @IsNotEmpty()
  @IsString()
  name: string;

  @IsOptional()
  @IsString()
  subtitle: string;

  @IsOptional()
  @IsString()
  notes: string;

  @IsNotEmpty()
  @IsArray()
  playerIds: number[];

  @IsNotEmpty()
  @IsNumber()
  phaseId: number;

  players?: Player[];
}

export class UpdateMatchDto {
  @IsNotEmpty()
  @IsString()
  name: string;

  @IsOptional()
  @IsString()
  subtitle: string;

  @IsOptional()
  @IsString()
  notes: string;

  @IsOptional()
  @IsArray()
  playerIds: number[];

  @IsOptional()
  @IsNumber()
  phaseId: number;

  players?: Player[];
  phase?: Phase;
}
