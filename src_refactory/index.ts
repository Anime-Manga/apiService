import { env, exit, platform } from "process";
import fs from "fs";
import path from "path";
import _ from "lodash";
import {config} from "dotenv";

//load env
config();

import fastifyServer from "./applications/serverFastify"
import Logger from "./applications/logger";

const logger = new Logger("server");
const __dirname = path.resolve();

async function init(){
    logger.info("Starting service api...");

    //routes
    logger.debug("Starting load routes");
    await loadRoutes();
    logger.debug("Finish load all routes");

    //server
    let connection = "";
    try{
        connection = await fastifyServer.listen({
            port: _.isNil(env.SERVER_PORT)? 3001 : parseInt(env.SERVER_PORT),
            host: env.SERVER_PORT || "127.0.0.1",
        })
    }catch(err){
        logger.fatal(`Stop service api reason:`, err);
        exit(1);
    }
    
    logger.info(`Connection established ${connection}`);
}

async function loadRoutes(){
    const listFileRoutes: Array<string> = [];

    try{
        listFileRoutes.push(...(fs.readdirSync(path.join(__dirname, "controllers"))).filter((file) => file.indexOf('.') !== -1));
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

init();