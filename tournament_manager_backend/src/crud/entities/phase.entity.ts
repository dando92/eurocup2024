import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany } from 'typeorm';
import { Match } from './match.entity'
import { Division } from './division.entity'

@Entity()
export class Phase {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @OneToMany(() => Match, (match) => match, { eager: true })
  matches: Match[];

  @ManyToOne(() => Division, (round) => round.phases)
  division: Division;
}