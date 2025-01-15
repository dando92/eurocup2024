import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany } from 'typeorm';
import { Match } from './match.entity'
import { Standing } from './standing.entity'
import { Song } from './song.entity'

@Entity()
export class Round {
  @PrimaryGeneratedColumn()
  id: number;

  @OneToMany(() => Standing, (standing) => standing.round)
  standings: Standing[]
  
  @ManyToOne(() => Match, (round) => round.rounds)
  match: Match;

  @ManyToOne(() => Song, (song) => song.rounds)
  song: Song;
}
