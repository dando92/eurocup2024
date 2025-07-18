// Suggested code may be subject to a license. Learn more: ~LicenseLog:2556429189.
import { ApiProperty } from '@nestjs/swagger';
import { IsNotEmpty, IsNumber, IsOptional } from 'class-validator';
import { Match, Song }  from '../entities';

export class CreateRoundDto {
  @ApiProperty({ description: 'The ID of the match this round belongs to' })
  @IsNotEmpty()
  @IsNumber()
  matchId: number;

  @ApiProperty({ description: 'The ID of the song played in this round' })
  @IsNotEmpty()
  @IsNumber()
  songId: number;

  @ApiProperty({ description: 'list of disabled players' })
  @IsOptional()
  disabledPlayerIds: number[];
}

export class UpdateRoundDto {
  @ApiProperty({
    description: 'The ID of the match this round belongs to',
    required: false,
  })
  @IsOptional()
  @IsNumber()
  matchId: number;

  @ApiProperty({ description: 'The ID of the song played in this round', required: false })
  @IsOptional()
  @IsNumber()
  songId: number;

  @ApiProperty({ description: 'list of disabled players' })
  @IsOptional()
  disabledPlayerIds: number[];
  
  match?: Match;
  song?: Song;
}
