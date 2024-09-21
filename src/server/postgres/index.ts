import { DataSource } from "typeorm";

import Logger from "../../modules/logger";
const logger = new Logger("pg");

import path from "path";

let pg: DataSource;

async function init({
    address = "localhost",
    port = "5432",
    username = "root",
    password = "root",
    database = "animemanga"
}){
    pg = new DataSource({
        type: "postgres",
        host: address,
        port: parseInt(port),
        username,
        password,
        database,
        entities: [`${path.resolve()}/domain/models/*.ts`],
        synchronize: process.env.NODE_ENV === "dev",
        connectTimeoutMS: 5000
    });

    try {
        pg = await pg.initialize();
    } catch (err){
        logger.error("Error initialize postgres, reason:", err);
        return;
    }

    logger.debug("String connection:", `${address}:${port}`);
    logger.info("Connected to pg!");
}

export {
    pg,
    init
};