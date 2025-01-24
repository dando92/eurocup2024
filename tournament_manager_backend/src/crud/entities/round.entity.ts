import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany, JoinColumn } from 'typeorm';
import { Match } from './match.entity'
import { Standing } from './standing.entity'
import { Song } from './song.entity'

@Entity()
export class Round {
  @PrimaryGeneratedColumn()
  id: number;

  @OneToMany(() => Standing, (standing) => standing.round, { eager: true, cascade: true })
  standings: Standing[]
  
  @ManyToOne(() => Match, (match) => match.rounds, { onDelete: 'CASCADE' })
  match: Match;

  @ManyToOne(() => Song, (song) => song.rounds, { onDelete: 'CASCADE', eager: true })
  song: Song;
}
