import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, OneToMany, ManyToMany, JoinTable } from 'typeorm';
import { Round } from './round.entity'
import { Phase } from './phase.entity'
import { Player } from './player.entity'

@Entity()
export class Match {
  @PrimaryGeneratedColumn()
  id: number;
  
  @Column()
  name: string;

  @Column()
  subtitle: string;

  @Column()
  notes: string;
  
  // public double Multiplier { get; set; } = 1;
  // public bool IsManualMatch { get; set; }
  // public string ScoringSystem { get; set; }
  
  @ManyToMany(() => Player)
  @JoinTable()
  players: Player[];

  @OneToMany(() => Round, (round) => round.match)
  matches: Round[];

  @ManyToOne(() => Phase, (round) => round.matches)
  phase: Phase;
}