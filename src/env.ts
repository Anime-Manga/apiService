import _ from "lodash";
import { existsSync } from "fs";
import { logger } from "./logger.js";

if(existsSync(".env")){
    process.loadEnvFile();
}

export const DATABASE_URL = process.env.DATABASE_URL as string;

if(_.isNil(DATABASE_URL) || DATABASE_URL === ""){
    logger.error("Required DATABASE_URL");
    process.exit(1);
}