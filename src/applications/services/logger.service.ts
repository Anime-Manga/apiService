
import { Injectable, LoggerService } from '@nestjs/common';

import loggerUtils from '../../utils/logger.utils.js';

@Injectable()
export class NestPinoLogger implements LoggerService {
  private readonly logger = loggerUtils.getLogger('nestjs');

  log(message: any): void {
    this.write('info', message);
  }

  error(message: any): void {
    this.write('error', message);
  }

  warn(message: any): void {
    this.write('warn', message);
  }

  debug(message: any): void {
    this.write('debug', message);
  }

  verbose(message: any): void {
    this.write('trace', message);
  }

  fatal(message: any): void {
    this.write('fatal', message);
  }

  private write(
    level: 'info' | 'error' | 'warn' | 'debug' | 'trace' | 'fatal',
    message: any
  ): void {
    
    const text = typeof message === 'string' ? message : JSON.stringify(message);

    this.logger[level](text ?? String(message));
  }
}
