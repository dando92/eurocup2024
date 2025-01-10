import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ScoresController } from './controllers/scores.controller'
import { ScoresService } from './services/scores.service'

import { Division } from './entities/division.entity';
import { Score } from './entities/score.entity';
import { Player } from './entities/player.entity';
import { Song } from './entities/song.entity';
import { Standing } from './entities/standing.entity';
import { Round } from './entities/round.entity';
import { Match } from './entities/match.entity';
import { Phase } from './entities/phase.entity';
import { Tournament } from './entities/tournament.entity';
import { Team } from './entities/team.entity';

@Module({
  // imports: [
  //   TypeOrmModule.forRoot({
  //     type: 'sqlite',
  //     database: 'prova.db',
  //     entities: [Player, Song, Score, Standing,  Round, Match, Match, Phase, Division],
  //     synchronize: true,
  //   }),
  //   TypeOrmModule.forFeature([Player, Song, Score, Standing,  Round, Match, Phase, Division]),
  //   SongsModule,
  //   DivisionsModule,
  //   PhasesModule,
  //   MatchesModule,
  //   RoundsModule,
  //   PlayersModule,
  //   ScoresModule,
  //   StandingsModule
  // ],
  // controllers: [AppController, SongsController],
  // providers: [AppService, SongsService, PlayersService],

  imports: [
    TypeOrmModule.forRoot({
      type: 'sqlite',
      database: 'tournament.db',
      entities: [Player, Song, Score, Standing,  Round, Match, Match, Phase, Division, Tournament, Team],
      synchronize: true,
    }),
    TypeOrmModule.forFeature([Player, Song, Score, Standing,  Round, Match, Phase, Division, Tournament, Team]),
  ],
  controllers: [ScoresController],
  providers: [ScoresService],
})
export class AppModule {}
