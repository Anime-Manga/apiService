import { PATH_BASE_CONTROLLER, schemaCreateAccount, schemaReturnAccount } from "../../schemas/accountSchema";
import { SCHEMA_BAD_REQUEST, SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

fastify.post(`${PATH_BASE_CONTROLLER}/create`, {
    schema: {
        tags: ["account"],
        summary: "Create new account",
        body: schemaCreateAccount,
        response: {
            200: schemaReturnAccount,
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            401: { $ref: SCHEMA_BAD_REQUEST }
        }
    }
}, async (request, replay) => replay.status(200).send(await accountService.createAccount(request.body)));