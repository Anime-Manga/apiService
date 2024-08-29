import Joi from "joi";

import { AccountService } from "../../../applications/services/AccountService";

import { IRoute } from "../../../server/fastify/interfaces/IRoute";
import { IAccount, IAccountEnv } from "../../../domain/interfaces/models/IAccount";

const accountService = new AccountService();

export default {
    method: "POST",
    url: "/create",
    hander: async function (request, replay){
        const account = request.body as IAccount;

        return accountService.createAccount(account);
    },
    options: {
        schema: {
            body: Joi.object({
                [IAccountEnv.USERNAME]: Joi.string().max(250).required(),
                [IAccountEnv.PASSWORD]: Joi.string().max(500).required(),
                [IAccountEnv.CHANGE_PASSWORD]: Joi.boolean().required()
            })
        }
    }
} as IRoute