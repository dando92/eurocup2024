import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany } from 'typeorm';
import { Score } from './score.entity'
import { Team } from './team.entity'

@Entity()
export class Player {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @OneToMany(() => Score, (score) => score.player, { eager: true })
  scores: Score[]

  @ManyToOne(() => Team, (team) => team.players, { eager: true })
  team: Team;
}

