import { IsNotEmpty, IsNumber, IsOptional, IsBoolean } from 'class-validator';
import { Type } from 'class-transformer';

export class CreateScoreDto {
  @IsNotEmpty()
  @IsNumber()
  @Type(() => Number)
  percentage: number;

  @IsNotEmpty()
  @IsBoolean()
  @Type(() => Boolean)
  isFailed: boolean;

  @IsNotEmpty()
  @IsNumber()
  @Type(() => Number)
  songId: number;

  @IsNotEmpty()
  @IsNumber()
  @Type(() => Number)
  playerId: number;

  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  score: number;
}

export class UpdateScoreDto {
  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  percentage: number;

  @IsOptional()
  @IsBoolean()
  @Type(() => Boolean)
  isFailed: boolean;

  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  songId: number;

  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  playerId: number;

  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  score: number;
}
