import Joi from "joi";

import { AccountService } from "../../../applications/services/AccountService";

import { IRoute } from "../../../server/fastify/interfaces/IRoute";
import { IAccount, IAccountEnv } from "../../../domain/interfaces/models/IAccount";

const accountService = new AccountService();

export default {
    method: "GET",
    url: "/find",
    hander: async function (request, replay){
        const {username} = request.query as {username: string};

        return accountService.findFromUsername(username);
    },
    options: {
        schema: {
            querystring: Joi.object({
                [IAccountEnv.USERNAME]: Joi.string().max(250).required()
            })
        }
    }
} as IRoute