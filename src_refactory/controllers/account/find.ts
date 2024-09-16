import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_RETURN_ACCOUNT, SCHEMA_QUERY_USERNAME_ACCOUNT, staticQueryUsernameAccount } from "../../schemas/accountSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";
import { SCHEMA_ERROR } from "../../schemas/generic/genericSchema";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        querystring: { $ref: SCHEMA_QUERY_USERNAME_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_ERROR },
            404: { $ref: SCHEMA_ERROR },
        }
    }
}

fastify.get<{Querystring: staticQueryUsernameAccount}>(`${PATH_BASE_CONTROLLER}/find`, options, async (request) => {
    const {username} = request.query;

    return await accountService.findFromUsername(username);
})