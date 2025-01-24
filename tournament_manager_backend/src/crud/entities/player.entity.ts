import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany, ManyToMany, JoinColumn } from 'typeorm';
import { Score } from './score.entity'
import { Team } from './team.entity'
import { Match } from './match.entity';

@Entity()
export class Player {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @OneToMany(() => Score, (score) => score.player, { eager: true, cascade: true })
  scores: Score[]

  @ManyToOne(() => Team, (team) => team.players)
  @JoinColumn()
  team: Team;
  
  @ManyToMany(() => Match, (match) => match.players)
  matches: Match[];
}

