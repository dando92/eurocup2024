// Suggested code may be subject to a license. Learn more: ~LicenseLog:175172732.
import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Entities  } from './crud/entities';
import { Services } from './crud/services';
import { Controllers } from './crud/controllers';

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
  controllers: Controllers,
  providers: Services,
})
export class AppModule {}
