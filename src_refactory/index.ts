import {config} from "dotenv";

import {init} from "./applications/serverFastify"

//load env
config();

init();