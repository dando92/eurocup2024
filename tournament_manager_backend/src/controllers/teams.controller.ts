import { Controller, Get, Post, Body, Patch, Param, Delete, ValidationPipe } from '@nestjs/common';
import { TeamsService } from '../services/teams.service ';
import { CreateTeamDto, UpdateTeamDto } from '../dtos/team.dto';

@Controller('teams')
export class TeamsController {
  constructor(private readonly scoresService: TeamsService) { }

  @Post()
  create(@Body(new ValidationPipe()) createScoreDto: CreateTeamDto) {
    return this.scoresService.create(createScoreDto);
  }

  @Get()
  findAll() {
    return this.scoresService.findAll();
  }

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.scoresService.findOne(+id);
  }

  @Patch(':id')
  update(@Param('id') id: string, @Body(new ValidationPipe()) updateScoreDto: UpdateTeamDto) {
    return this.scoresService.update(+id, updateScoreDto);
  }

  @Delete(':id')
  remove(@Param('id') id: string) {
    return this.scoresService.remove(+id);
  }
}
