import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND, schemaReturnVoid } from "../../schemas/generic/genericSchema";
import { PATH_BASE_CONTROLLER} from "../../schemas/accountSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";
import { schemaQueryUsernameAuth } from "../../schemas/generic/authSchema";

const accountService = new AccountService();

fastify.delete(`${PATH_BASE_CONTROLLER}/delete`, {
    schema: {
        tags: ["account"],
        summary: "Delete account",
        querystring: schemaQueryUsernameAuth,
        response: {
            200: schemaReturnVoid,
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND }
        }
    }
}, async (request, replay) => {
    const {auth_username} = request.query;

    await accountService.deleteAccount(auth_username);

    replay.status(200).send({ response: "ok" });
});