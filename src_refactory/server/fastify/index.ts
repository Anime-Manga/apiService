import { env, exit, platform } from "process";
import fs from "fs";
import Fastify from 'fastify';
import { DateTime } from 'luxon';
import path from "path";
import _ from "lodash";

import { ApiConflict, ApiErrorGeneric, ApiNotFound, ApiUnauthorized } from "../../modules/api";

import { IController } from "./interfaces/IController";

import Logger from '../../modules/logger';
const logger = new Logger("server-fastify");

const __dirname = path.resolve();

//settings
const fastify = Fastify();

//init
async function init(pathControllers = "controllers"){
    logger.info("Starting service fastify...");

    //controllers
    logger.debug("Starting load controllers");
    await loadControllers(pathControllers);
    logger.debug("Finish load all controllers");

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
async function loadControllers(path_controllers: string){
    const listFileControllers: Array<string> = [];
    const basePathController = path.join(__dirname, path_controllers);

    //get list folder inside controllers
    const listControllers = fs.readdirSync(basePathController).map((singlePath) => path.join(path_controllers, singlePath));
    
    //get index every controller
    for (const controller of listControllers) {
        try{
            listFileControllers.push(...(fs.readdirSync(controller)).filter((file) => file === "index.ts").map((singlePath) => path.join(controller, singlePath)));
        }catch(err){
            logger.error("Failed load controllers reason:", err);
        } 
    }

    logger.debug("List controllers available:", listFileControllers.join(', '))
    
    let success = 0;
    for (const fileController of listFileControllers) {
        let controller: IController;

        try{
            controller = (await import(path.join(platform === "win32"? 'file://' : '', __dirname, fileController))).default;
        }catch(err){
            logger.error("Failed load", `"${fileController}", details:`, err);
            continue;
        }

        loadRoutes(controller, fileController);

        success++;
    }

    logger.debug('Total controllers loaded', success);
}

function loadRoutes(controller: IController, nameController: string){
    logger.debug("Routes", `(${Object.keys(controller.routes).length})`, "available");

    let success = 0;
    for (const route in controller.routes) {
        const pathRoute = path.join(controller.pathBaseController, controller.routes[route].url);
        try{
            fastify.route({
                url: pathRoute,
                method: controller.routes[route].method,
                handler: controller.routes[route].hander,
                validatorCompiler: ({ schema }) => {
                    //@ts-ignore
                    return data => schema.validate(data)
                },
                ...controller.routes[route].options
            })
        }catch(err){
            logger.error("Failed load route", `"${pathRoute}" details:`, err);
            continue;
        }
        
        success++;
    }

    logger.debug('Loaded', `(${success}) routes from`, nameController);
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
    }else if(replay.statusCode !== 200){
        logger.warn(...message);
    }else{
        logger.info(...message);
    }

    next();
})

fastify.setErrorHandler((error, request, replay) => {
    const message = {
        error: error.name,
        message: error.message
    }

    if(error instanceof ApiConflict){
        return replay.status(409).send({
            ...message,
            statusCode: 409
        })
    }else if(error instanceof ApiNotFound){
        return replay.status(404).send({
            ...message,
            statusCode: 404
        });
    }else if(error instanceof ApiUnauthorized){
        return replay.status(401).send({
            ...message,
            statusCode: 401
        });
    }else if(error instanceof ApiErrorGeneric){
        return replay.status(500).send({
            ...message,
            statusCode: 500
        });
    }else{
        logger.error(error);
        return error;
    }
})

export default fastify;

export {
    init
}