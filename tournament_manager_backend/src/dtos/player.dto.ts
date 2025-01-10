import { PartialType } from '@nestjs/mapped-types';

export class CreatePlayerDto {
  name: string;
  teamId: number;
}

export class UpdatePlayerDto extends PartialType(CreatePlayerDto) {
}
