import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_RETURN_ACCOUNT, SCHEMA_UPDATE_ACCOUNT, SCHEMA_QUERY_USERNAME_ACCOUNT, staticQueryUsernameAccount, staticUpdateAccount } from "../../schemas/accountSchema";

import { AccountService } from "../../applications/services/AccountService";

import { IAccount } from "../../domain/interfaces/models/IAccount";
import fastify from "../../server/fastify";
import { SCHEMA_ERROR } from "../../schemas/generic/genericSchema";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        querystring: { $ref: SCHEMA_QUERY_USERNAME_ACCOUNT },
        body: { $ref: SCHEMA_UPDATE_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_ERROR },
            404: { $ref: SCHEMA_ERROR },
        }
    }
}

fastify.put<{Querystring: staticQueryUsernameAccount, Body: staticUpdateAccount}>(`${PATH_BASE_CONTROLLER}/update`, options, async (request) => {
    const {username} = request.query;
    const account = request.body;

    return await accountService.updateAccount(username, account);
})