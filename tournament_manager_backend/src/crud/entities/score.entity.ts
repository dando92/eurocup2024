import { Entity, Column, PrimaryGeneratedColumn, ManyToOne } from 'typeorm';
import { Song } from './song.entity'
import { Player } from './player.entity'

@Entity()
export class Score {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  percentage: number;

  @Column()
  score: number;

  @Column()
  isFailed: boolean;

  @ManyToOne(() => Song, (song) => song.scores, { cascade: true })
  song: Song

  @ManyToOne(() => Player, (player) => player.scores, { cascade: true })
  player: Player
}
