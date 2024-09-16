import { RouteShorthandOptions } from "fastify";

import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND, SCHEMA_RETURN_VOID } from "../../schemas/generic/genericSchema";
import { PATH_BASE_CONTROLLER, SCHEMA_QUERY_USERNAME_ACCOUNT, staticQueryUsernameAccount } from "../../schemas/accountSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        tags: ['account'],
        summary: 'Delete account',
        querystring: { $ref: SCHEMA_QUERY_USERNAME_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_VOID },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND },
        }
    }
}

fastify.delete<{Querystring: staticQueryUsernameAccount}>(`${PATH_BASE_CONTROLLER}/delete`, options, async (request) => {
    const {username} = request.query;

    await accountService.deleteAccount(username);

    return { response: "ok" }
})