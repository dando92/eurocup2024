import { Controller, Get, Post, Body, Patch, Param, Delete } from '@nestjs/common';
import { PlayerService } from 'src/services/players.service';
import { CreatePlayerDto, UpdatePlayerDto } from 'src/dtos/player.dto';

@Controller('players')
export class PlayersController {
  constructor(private readonly service: PlayerService) {}

  @Post()
  create(@Body() createScoreDto: CreatePlayerDto) {
    return this.service.create(createScoreDto);
  }

  @Get()
  findAll() {
    return this.service.findAll();
  }

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.service.findOne(+id);
  }

  @Patch(':id')
  update(@Param('id') id: string, @Body() updateScoreDto: UpdatePlayerDto) {
    return this.service.update(+id, updateScoreDto);
  }

  @Delete(':id')
  remove(@Param('id') id: string) {
    return this.service.remove(+id);
  }
}
