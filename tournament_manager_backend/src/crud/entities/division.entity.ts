import { Entity, Column, PrimaryGeneratedColumn, OneToMany, ManyToOne } from 'typeorm';
import { Phase } from './phase.entity'
import { Tournament } from './tournament.entity';

@Entity()
export class Division {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @OneToMany(() => Phase, (phase) => phase, { eager: true })
  phases: Phase[];

  @ManyToOne(() => Tournament, (tournament) => tournament.divisions)
  tournament: Tournament;
}

