import { Entity, Column, PrimaryGeneratedColumn, OneToMany } from 'typeorm';
import { Division } from './division.entity'

@Entity()
export class Tournament {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  name: string;

  @OneToMany(() => Division, (division) => division.tournament)
  divisions: Division[]
}
