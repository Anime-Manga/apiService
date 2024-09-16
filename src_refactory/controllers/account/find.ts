import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_RETURN_ACCOUNT, SCHEMA_QUERY_USERNAME_ACCOUNT, staticQueryUsernameAccount } from "../../schemas/accountSchema";
import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        tags: ['account'],
        summary: 'Find account',
        querystring: { $ref: SCHEMA_QUERY_USERNAME_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND },
        }
    }
}

fastify.get<{Querystring: staticQueryUsernameAccount}>(`${PATH_BASE_CONTROLLER}/find`, options, async (request) => {
    const {username} = request.query;

    return await accountService.findFromUsername(username);
})