import Joi from "joi";

import { IRoute } from "../../../server/fastify/interfaces/IRoute";
import { IAccountEnv } from "../../../domain/interfaces/models/IAccount";

import { AccountService } from "../../../applications/services/AccountService";
const accountService = new AccountService();

export default {
    method: "DELETE",
    url: "/delete",
    hander: async function (request, replay){
        const {username} = request.query as {username: string};

        return accountService.deleteAccount(username);
    },
    options: {
        schema: {
            querystring: Joi.object({
                [IAccountEnv.USERNAME]: Joi.string().required()
            })
        }
    }
} as IRoute