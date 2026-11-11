import { MiddlewareConsumer, Module, NestModule, RequestMethod } from '@nestjs/common';

import { AccountsModule } from './applications/modules/accounts.module.js';
import { loggerMiddleware } from './middlewares/logger.middleware.js';

@Module({
  imports: [
    AccountsModule
  ],
  providers: [loggerMiddleware]
})
export class AppModule implements NestModule {
  configure(consumer: MiddlewareConsumer) {
    consumer
      .apply(loggerMiddleware)
      .forRoutes("*")
  }

}
