import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_CREATE_ACCOUNT, SCHEMA_RETURN_ACCOUNT, staticCreateAccount } from "../../schemas/accountSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";
import { SCHEMA_ERROR } from "../../schemas/generic/genericSchema";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        body: { $ref: SCHEMA_CREATE_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_ERROR },
            401: { $ref: SCHEMA_ERROR },
        }
    }
}

fastify.post<{Querystring: staticCreateAccount}>(`${PATH_BASE_CONTROLLER}/create`, options, async (request) => {
    return await accountService.createAccount(request.body);
})