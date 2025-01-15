import { Get, Post, Body, Patch, Param, Delete, ValidationPipe } from '@nestjs/common';
import { ICrudService } from '../services/ICrudService';

export class GenericController<TEntity, TCreateDto, TUpdateDto, TService extends ICrudService<TEntity, TCreateDto, TUpdateDto>> {
  constructor(private readonly service: TService) { }

  @Post()
  async create(@Body(new ValidationPipe()) dto: TCreateDto): Promise<TEntity> {
    return await this.service.create(dto);
  }

  @Get()
  async findAll(): Promise<TEntity[]> {
    return await this.service.findAll();
  }

  @Get(':id')
  findOne(@Param('id') id: string): Promise<TEntity | null> {
    return this.service.findOne(+id);
  }

  @Patch(':id')
  update(@Param('id') id: string, @Body(new ValidationPipe()) dto: TUpdateDto): Promise<TEntity> {
    return this.service.update(+id, dto);
  }

  @Delete(':id')
  remove(@Param('id') id: string): Promise<void> {
    return this.service.remove(+id);
  }
}
