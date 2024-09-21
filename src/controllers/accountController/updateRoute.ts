import { PATH_BASE_CONTROLLER, schemaQueryUsernameAccount, schemaUpdateAccount, schemaReturnAccount } from "../../schemas/accountSchema";
import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

fastify.put(`${PATH_BASE_CONTROLLER}/update`, {
    schema: {
        tags: ["account"],
        summary: "Update values of account",
        querystring: schemaQueryUsernameAccount,
        body: schemaUpdateAccount,
        response: {
            200: schemaReturnAccount,
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND }
        }
    }
}, async (request, replay) => {
    const {username} = request.query;
    const account = request.body;

    replay.status(200).send(await accountService.updateAccount(username, account));
});