import {config} from "dotenv";

import {init as initServerFastify} from "./applications/serverFastify"
import {init as initPostgres} from "./applications/postgres"

//load env
config();

await initPostgres(null, null, process.env.POSTGRES_USERNAME, process.env.POSTGRES_PASSWORD);

await initServerFastify();