import fs from "fs";
import _ from "lodash";
import path from "path";
import Fastify from 'fastify';
import { DateTime } from 'luxon';
import { env, exit } from "process";

import { IConfig } from "./interfaces/IConfig";
import { ApiBadRequest, ApiConflict, ApiErrorGeneric, ApiNotFound, ApiUnauthorized } from "../../modules/api";

import Logger from '../../modules/logger';
const logger = new Logger("server-fastify");

const __dirname = path.resolve();

//settings
const fastify = Fastify();

//init
async function init({pathControllers = "controllers", pathSchemas = "schemas", swagger = true}: IConfig){
    logger.info("Starting service fastify...");

    //swagger
    if(swagger){
        logger.debug("Starting load swagger");
        await fastify.register(await import("@fastify/swagger"));
        await fastify.register(await import('@fastify/swagger-ui'), {
            routePrefix: '/',
            uiConfig: {
                docExpansion: 'list',
                deepLinking: false
            },
            uiHooks: {
                onRequest: function (request, reply, next) { next() },
                preHandler: function (request, reply, next) { next() }
            },
            logLevel: "error"
        })
        logger.debug("Finish load swagger");
    }

    //controllers
    logger.debug("Starting load controllers");
    await loadControllers(pathControllers);
    logger.debug("Finish load all controllers");

    //schemas
    logger.debug("Starting load schemas");
    await loadSchemas(pathSchemas);
    logger.debug("Finish load all schemas");

    await fastify.ready()
    fastify.swagger()

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

//load controllers
async function loadControllers(pathControllers: string){
    const basePathController = path.join(__dirname, pathControllers);

    //get list folder inside controllers
    let listFileControllers: Array<string> = [];
    try{
        listFileControllers = fs.readdirSync(basePathController).filter((file) => file.indexOf('.') === -1).map((folder) => path.join(basePathController, folder));
    }catch(err){
        logger.fatal("Cannot read folder", pathControllers, "details: ", err);
        exit(1);
    }
    
    logger.debug("List controllers available:", listFileControllers.map((file) => path.basename(file)));
    logger.debug("Controllers", `(${listFileControllers.length})`, "available");
    
    let success = 0;
    for (const fileController of listFileControllers) {
        const totalRoutes = await loadRoutes(fileController);

        if(totalRoutes > 0){
            logger.debug("Done import controller", path.basename(fileController));
            success++;
        }else{
            logger.error("Controller", path.basename(fileController), "maybe has some error");
        }
    }

    if(success === 0){
        logger.fatal("controllers none loaded");
        exit(1);
    }

    logger.debug('Total controllers loaded', success);
}

//load routes
async function loadRoutes(pathController: string){
    let listFileRoutes: Array<string> = [];

    try{
        listFileRoutes = fs.readdirSync(pathController).filter((file) => file.indexOf(".") !== -1).map((file) => path.join(pathController, file));
    }catch(err){
        logger.error("Cannot read controller", path.basename(pathController), "details:", err);
    }

    logger.debug("List routes available", listFileRoutes.map((file) => path.basename(file)));
    logger.debug("Routes", `(${listFileRoutes.length})`, "available");

    let success = 0;
    for (const fileRoute of listFileRoutes) {
        try{
            await import(fileRoute);
        }catch(err){
            logger.error("Failed load route", `"${fileRoute}" details:`, err);
            continue;
        }

        logger.debug("Done import route", path.basename(fileRoute));
        success++;
    }

    logger.debug('Loaded', `(${success}) routes from`, path.basename(pathController));
    return success;
}

//load schemas
async function loadSchemas(pathSchemas){
    const basePathSchemas = path.join(__dirname, pathSchemas);

    if(!fs.existsSync(basePathSchemas)){
        logger.warn("Not exist root folder schemas, skip schemas")
        return;
    }

    //get list folder inside controllers
    const listFileSchemas = fs.readdirSync(basePathSchemas, {recursive: true, withFileTypes: true}).filter((singlePath) =>  singlePath.isFile()).map((singlePath) => path.join(singlePath.parentPath, singlePath.name));

    logger.debug(`List schemas available: ${listFileSchemas.map((file) => path.basename(file))}`);

    let success = 0;
    let listSchemas: Array<any>;
    for (const fileSchema of listFileSchemas) {
        try{
            listSchemas = (await import(fileSchema)).default;
        }catch(err){
            logger.error(`Failed load schema "${path.basename(fileSchema)}", details:`, err);
            continue;
        }

        if(!_.isNil(listSchemas)){
            for (const schema of listSchemas) {
                try{
                    fastify.addSchema(schema)
                }catch(err){
                    logger.error(`Failed add specific schema of ${schema['$id']}, details:`, err);
                }
            }
        }

        logger.debug("Done import schema", path.basename(fileSchema));
        success++;
    }

    logger.debug("Total schemas loaded", success);
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
    }else if(error instanceof ApiBadRequest){
        return replay.status(400).send({
            ...message,
            statusCode: 400
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