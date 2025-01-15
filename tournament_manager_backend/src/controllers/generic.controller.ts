import { Get, Post, Body, Patch, Param, Delete, ValidationPipe } from '@nestjs/common';
import { CreateDivisionDto, UpdateDivisionDto } from 'src/dtos/division.dto';
import { ICrudService } from 'src/interface/ICrudService';


export class GenericController<TService extends ICrudService<any, any, any>> {
  constructor(private readonly service: TService) { }

  @Post()
  create(@Body(new ValidationPipe()) createDivisionDto: CreateDivisionDto) {
    return this.service.create(createDivisionDto);
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
  update(@Param('id') id: string, @Body(new ValidationPipe()) updateDivisionDto: UpdateDivisionDto) {
    return this.service.update(+id, updateDivisionDto);
  }

  @Delete(':id')
  remove(@Param('id') id: string) {
    return this.service.remove(+id);
  }
}
