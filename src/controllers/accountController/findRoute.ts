import { PATH_BASE_CONTROLLER, schemaQueryUsernameAccount, schemaReturnAccount } from "../../schemas/accountSchema";
import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

fastify.get(`${PATH_BASE_CONTROLLER}/find`, {
    schema: {
        tags: ["account"],
        summary: "Find account",
        querystring: schemaQueryUsernameAccount,
        response: {
            200: schemaReturnAccount,
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND }
        }
    }
}, async (request, replay) => {
    const {username} = request.query;

    replay.status(200).send(await accountService.findFromUsername(username));
});