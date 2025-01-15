import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Phase } from '../entities/phase.entity';
import { Division } from '../entities/division.entity';
import { CreatePhaseDto, UpdatePhaseDto } from 'src/dtos/phase.dto';
import { ICrudService } from '../interface/ICrudService';

@Injectable()
export class PhasesService implements ICrudService<Phase, CreatePhaseDto, UpdatePhaseDto> {
  constructor(
    @InjectRepository(Phase)
    private phasesRepository: Repository<Phase>,
    @InjectRepository(Division)
    private divisionRepo: Repository<Division>,

  ) {}

  async create(phaseDto: CreatePhaseDto){
    const phase = new Phase();
    phase.name = phaseDto.name;
    this.phasesRepository.insert(phase);
    return phase;
  }

  async findAll(){
    return this.phasesRepository.find();
  }

  async findOne(id: number){
    return this.phasesRepository.findOneBy({ id });
  }

  async update(id: number, dto: UpdatePhaseDto) {
    const phase = await this.phasesRepository.findOneBy({ id });

    if (!phase) {
      throw Error(`Phase with id ${id} not found. Update phase failed`);
    }

    if (dto.divisionId) {
      const division = await this.divisionRepo.findOneBy({ id: dto.divisionId });
      if (!division) {
        throw Error(`Division with id ${dto.divisionId} not found. Update phase failed`);
      }
      dto.division = division;
      delete dto.divisionId;
    }

    await this.phasesRepository.merge(phase, dto);

    return phase;
  }

  async remove(id: number): Promise<void> {
    await this.phasesRepository.delete(id);
  }
}
