import Joi from "joi";

import { IRoute } from "../../../server/fastify/interfaces/IRoute";
import { IAccount, IAccountEnv } from "../../../domain/interfaces/models/IAccount";

import { AccountService } from "../../../applications/services/AccountService";
const accountService = new AccountService();

export default {
    method: "PUT",
    url: "/update",
    hander: async function (request, replay){
        const {username} = request.query as {username: string};
        const account = request.body as IAccount;

        return accountService.updateAccount(username, account);
    },
    options: {
        schema: {
            querystring: Joi.object({
                [IAccountEnv.USERNAME]: Joi.string().required()
            }),
            body: Joi.object({
                [IAccountEnv.CHANGE_PASSWORD]: Joi.boolean().optional(),
                [IAccountEnv.PROFILE]: Joi.string().optional(),
            })
        }
    }
} as IRoute