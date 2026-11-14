import _ from "lodash";
import pino from "pino";
import { existsSync } from "fs";
import pretty from "pino-pretty";

if(existsSync(".env")){
    process.loadEnvFile();
}

const level = process.env.LOG_LEVEL?.toLowerCase() || "info";

if(_.isNil(pino.levels.values[level])){
    throw new Error(`not exist log level "${level}"`);
}

function getLogger(name: string | undefined): pino.Logger{
    return pino({
        level: process.env.LOG_LEVEL?.toLowerCase() || "info",
        transport: {
            target: "pino-pretty",
            options: {
                colorize: pretty.isColorSupported,
                singleLine: true,
                translateTime: 'dd/mm/yyyy HH:MM:ss.l'
            } satisfies pretty.PrettyOptions
        },
        name
    });
}

export default {
    getLogger
}