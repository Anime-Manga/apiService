import Logger from "../../modules/logger";
const logger = new Logger("account-controller");

logger.debug("Loaded controller!")

import * as routes from "./routes";
import { IController } from "../../server/fastify/interfaces/IController";

export default {
    routes,
    pathBaseController: "/account"
} as IController;