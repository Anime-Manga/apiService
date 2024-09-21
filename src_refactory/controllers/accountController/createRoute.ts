import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_CREATE_ACCOUNT, SCHEMA_RETURN_ACCOUNT, staticCreateAccount } from "../../schemas/accountSchema";
import { SCHEMA_BAD_REQUEST, SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

const options: RouteShorthandOptions = {
    schema: {
        tags: ["account"],
        summary: "Create new account",
        body: { $ref: SCHEMA_CREATE_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            401: { $ref: SCHEMA_BAD_REQUEST }
        }
    }
};

fastify.post<{Querystring: staticCreateAccount}>(`${PATH_BASE_CONTROLLER}/create`, options, async (request) => {
    return await accountService.createAccount(request.body);
});