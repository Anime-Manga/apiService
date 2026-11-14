import { DATABASE_URL } from './env.js';

import { NestFactory } from '@nestjs/core';
import { FastifyAdapter } from '@nestjs/platform-fastify';
import { StandardSchemaValidationPipe } from '@nestjs/common';

import { AppModule } from './app.module.js';
import postgresqlUtils from './utils/postgresql.utils.js';
import { NestPinoLogger } from './applications/services/logger.service.js';

async function bootstrap() {
  //connect to db
  await postgresqlUtils.initDatabase(DATABASE_URL)

  //instance app
  const app = await NestFactory.create(
    AppModule,
    new FastifyAdapter(),
    {
      bufferLogs: true,
      logger: ["debug"],
      routeConflictPolicy: {
        duplicate: 'error',
        shadow: 'warn'
      },
    }
  );
  
  app.useGlobalPipes(new StandardSchemaValidationPipe());
  app.useLogger(new NestPinoLogger())

  await app.listen(process.env.PORT ?? 3000);
  
}
await bootstrap();
