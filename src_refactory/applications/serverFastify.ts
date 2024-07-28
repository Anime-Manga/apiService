import Fastify from 'fastify';
import { DateTime } from 'luxon';
import { env } from 'process';
import _ from "lodash";

import Logger from './logger';
const logger = new Logger("api");

//settings
const fastify = Fastify();

fastify.addHook("onRequest", (req, replay, next) => {
    //@ts-ignores
    replay.startTime = DateTime.now().toISO();

    next();
})

fastify.addHook("onResponse", (req, replay, next) => {
    //@ts-ignore
    const time = DateTime.fromISO(replay.startTime);

    let bodyJson = {};

    if(env.LOG_LEVEL === "debug"){
        const body = req.body;

        if(!_.isObject(body)){
            try{
                bodyJson = JSON.parse(body as string);
            }catch{
                //ignore
            }
        }else{
            bodyJson = body;
        }
    }

    const message = [req.raw.method, req.url.split('?')[0], JSON.stringify(req.query), JSON.stringify(bodyJson), replay.statusCode, `(${DateTime.now().diff(time).toMillis()} ms)`];
    
    if(replay.statusCode === 500){
        logger.error(...message);
    }else{
        logger.info(...message);
    }

    next();
})

export default fastify;