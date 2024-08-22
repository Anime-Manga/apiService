import { env, exit, platform } from "process";
import fs from "fs";
import Fastify from 'fastify';
import { DateTime } from 'luxon';
import path from "path";
import _ from "lodash";

import Logger from './logger';
const logger = new Logger("server-fastify");

const __dirname = path.resolve();

//settings
const fastify = Fastify();

//init
async function init(path_controllers = "controllers"){
    logger.info("Starting service fastify...");

    //routes
    logger.debug("Starting load routes");
    await loadRouters(path_controllers);
    logger.debug("Finish load all routes");

    //server
    let connection = "";
    try{
        connection = await fastify.listen({
            port: _.isNil(env.SERVER_PORT)? 3001 : parseInt(env.SERVER_PORT),
            host: env.SERVER_PORT || "127.0.0.1",
        })
    }catch(err){
        logger.fatal(`Stop service fastify reason:`, err);
        exit(1);
    }
    
    logger.info(`Connection established ${connection}`);
}

//load routers
async function loadRouters(path_controllers: string){
    const listFileRoutes: Array<string> = [];

    try{
        listFileRoutes.push(...(fs.readdirSync(path.join(__dirname, path_controllers))).filter((file) => file.indexOf('.') !== -1));
    }catch(err){
        logger.error("Failed load routes reason:", err);
    }

    logger.debug("List routes available:", listFileRoutes.join(', '))
    
    let success = 0;
    for (const route of listFileRoutes) {
        try{
            await import(path.join(platform === "win32"? 'file://' : '', __dirname, 'controllers', route));
        }catch(err){
            logger.error("Failed load", route);
            continue;
        }

        success++;
    }

    logger.debug('Total routes loaded', success);
}

//logging
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

export {
    init
}