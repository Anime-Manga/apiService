import {config} from "dotenv";

import {init as initServerFastify} from "./server/fastify"
import {init as initPostgres} from "./server/postgres"

//load env
config();

await initPostgres({
    username: process.env.POSTGRES_USERNAME,
    password: process.env.POSTGRES_PASSWORD,
    database: process.env.POSTGRES_DB
});

await initServerFastify();