// Suggested code may be subject to a license. Learn more: ~LicenseLog:175172732.
import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Entities } from './crud/entities';
import { Services } from './crud/services';
import { Controllers } from './crud/controllers';
import { BackwardCompatibilityController } from './backwardcompatibility.controller';
import { MatchManager } from './services/match.manager';
import { StandingManager } from './services/standing.manager';
import { SongExtractor } from './services/song.extractor';
import { TournamentCache } from './services/tournament.cache';
import { MatchGateway } from './gateways/match.gateway'

const controllers = [
  Controllers[0],
  Controllers[1],
  Controllers[2],
  Controllers[3],
  Controllers[4],
  Controllers[5],
  Controllers[6],
  Controllers[7],
  Controllers[8],
  Controllers[9],
  Controllers[10],
  Controllers[11],
  Controllers[12],
  BackwardCompatibilityController
]

const services = [
  Services[0],
  Services[1],
  Services[2],
  Services[3],
  Services[4],
  Services[5],
  Services[6],
  Services[7],
  Services[8],
  Services[9],
  TournamentCache,
  SongExtractor,
  MatchManager,
  StandingManager,
  MatchGateway
]


@Module({
  imports: [
    TypeOrmModule.forRoot({
      type: 'sqlite',
      database: 'tournament.db',
      entities: Entities,
      synchronize: true,
    }),
    TypeOrmModule.forFeature(Entities),
  ],
  controllers: controllers,
  providers: services,
})
export class AppModule { }
