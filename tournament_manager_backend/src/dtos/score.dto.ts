import { PartialType } from '@nestjs/mapped-types';

export class CreateScoreDto {
  percentage: number;
  score: number;
  isFailed: boolean;
  songId: number; 
  playerId: number
}

export class UpdateScoreDto extends PartialType(CreateScoreDto) {
}
