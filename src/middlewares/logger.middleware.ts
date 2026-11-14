import _ from 'lodash';
import { Injectable, NestMiddleware } from "@nestjs/common";
import type { FastifyRequest, FastifyReply } from 'fastify';

import loggerUtils from "../utils/logger.utils.js";

const logger = loggerUtils.getLogger("middleware-route");

@Injectable()
export class loggerMiddleware implements NestMiddleware {
    use(req: FastifyRequest, res: FastifyReply['raw'], next: (error?: any) => void) {
        const startTime = new Date();

        res.on("finish", () => {
            const endTime = new Date();
            const duration = Number(endTime.getTime() - startTime.getTime());

            const message = `${req.ip} ${req.url?.toString()} ${JSON.stringify(_.isPlainObject(req.body)? req.body : {})} (${res.statusCode}) [${duration.toFixed(0)} ms]`;

            if(res.statusCode >= 500){
                logger.error(message);
            }else if(res.statusCode >= 400){
                logger.warn(message);
            }else{
                logger.info(message);
            }

        })

        next();
    }

}