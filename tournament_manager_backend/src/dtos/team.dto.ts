import { PartialType } from '@nestjs/mapped-types';

export class CreateTeamDto {
  name:string;
}

export class UpdateTeamDto extends PartialType(CreateTeamDto) {
}
