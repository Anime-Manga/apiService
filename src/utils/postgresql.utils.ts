import { PrismaPg } from "@prisma/adapter-pg";
import { PrismaClient } from "../domain/prisma/client.js";

import { DefaultArgs } from "@prisma/client/runtime/client";
import { GlobalOmitConfig } from "../domain/prisma/internal/prismaNamespace.js";

import { logger } from "../logger.js";
import _ from "lodash";
import { HttpException, HttpStatus } from "@nestjs/common";

let client: PrismaClient<"query" | "info" | "warn" | "error", GlobalOmitConfig, DefaultArgs> | undefined;

async function initDatabase(url: string){
    const adapter = new PrismaPg({
      connectionString: url,
      connectionTimeoutMillis: 5000,
      log: (...message) => logger.debug(message),
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
    client.$on("query", (query) => logger.debug(`${query.query} (${query.duration} ms)`));
    client.$on("error", (error) => logger.error(error.message));
    client.$on("info", (info) => logger.debug(info.message));
    client.$on("warn", (warn) => logger.debug(warn.message));

    try {
      await client.$connect();
      await client.$queryRaw`SELECT 1`;
    } catch (err){
        logger.error(["Error connect postgres, reason:", err]);

        await client.$disconnect();
        process.exit(1);
    }
    
    await client.$disconnect();

    //logger.debug(`String connection: ${username}:${password}@${address}:${port}`);
    //logger.info(`Connected! use DB: ${database} --> ${username}:[HIDE PASSWORD]@${address}:${port}`);
    logger.info("ok");
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