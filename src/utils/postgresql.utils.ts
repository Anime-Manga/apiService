import _ from "lodash";
import { PrismaPg } from "@prisma/adapter-pg";
import { DefaultArgs } from "@prisma/client/runtime/client";
import { HttpException, HttpStatus } from "@nestjs/common";

import loggerUtils from "./logger.utils.js";
import { PrismaClient } from "../domain/prisma/client.js";
import { GlobalOmitConfig } from "../domain/prisma/internal/prismaNamespace.js";

let client: PrismaClient<"query" | "info" | "warn" | "error", GlobalOmitConfig, DefaultArgs> | undefined;

const logger = loggerUtils.getLogger("postgresql");

async function initDatabase(url: string){
    const adapter = new PrismaPg({
      connectionString: url,
      connectionTimeoutMillis: 5000,
      log: (...message) => logger.debug(message.join()),
    });

    client = new PrismaClient({
      adapter,
      log: [
        {
          emit: "event",
          level: "query",
        },
        {
          emit: "event",
          level: "error",
        },
        {
          emit: "event",
          level: "info",
        },
        {
          emit: "event",
          level: "warn",
        },
      ],
    });

    //set logs
    client.$on("query", (query) => logger.debug(`${query.query} (${query.duration.toFixed(0)} ms)`));
    client.$on("error", (error) => logger.error(error.message));
    client.$on("info", (info) => logger.info(info.message));
    client.$on("warn", (warn) => logger.warn(warn.message));

    try {
      await client.$connect();
      await client.$queryRaw`SELECT 1`;
    } catch (err){
        logger.error(`Error connect postgres, reason: ${err}`);

        await client.$disconnect();
        process.exit(1);
    }
    
    await client.$disconnect();

    logger.debug(`String connection: ${url}`);
    logger.info(`Connected! use DB: ${url}`);
}

function getClient(){
  if(_.isNil(client)){
    throw new HttpException("Client PG is not initiate yet", HttpStatus.INTERNAL_SERVER_ERROR);
  }

  return client;
}

export default {
  initDatabase,
  getClient
};