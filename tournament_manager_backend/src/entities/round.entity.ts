import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany } from 'typeorm';
import { Match } from './match.entity'
import { Standing } from './standing.entity'

@Entity()
export class Round {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  songId: number;

  @OneToMany(() => Standing, (standing) => standing.round)
  standings: Standing[]

  @ManyToOne(() => Match, (round) => round.matches)
  match: Match;
}
