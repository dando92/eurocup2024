import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToOne, JoinColumn } from 'typeorm';
import { Score } from './score.entity'
import { Round } from './round.entity'

@Entity()
export class Standing {
  @PrimaryGeneratedColumn()
  id: number;

  @OneToOne(() => Score, { cascade: true })
  @JoinColumn()
  score: Score

  @ManyToOne(() => Round, (round) => round.standings, { cascade: true })
  round: Round
}
