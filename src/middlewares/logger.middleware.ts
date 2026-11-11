import { Injectable, NestMiddleware } from "@nestjs/common";
import { IncomingMessage, ServerResponse } from "node:http";
import { logger } from "../logger.js";

@Injectable()
export class loggerMiddleware implements NestMiddleware {
    use(req: IncomingMessage, res: ServerResponse, next: (error?: any) => void) {
        const startTime = new Date();

        res.on("finish", () => {
            const endTime = new Date();
            const duration = Number(endTime.getTime() - startTime.getTime());

            logger.info(`${req.url?.toString()} (${res.statusCode}) [${duration.toFixed(0)} ms]`);
        })
        

        next();
    }

}