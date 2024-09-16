import { RouteShorthandOptions } from "fastify";

import { PATH_BASE_CONTROLLER, SCHEMA_RETURN_ACCOUNT, SCHEMA_UPDATE_ACCOUNT, SCHEMA_QUERY_USERNAME_ACCOUNT, staticQueryUsernameAccount, staticUpdateAccount } from "../../schemas/accountSchema";
import { SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND } from "../../schemas/generic/genericSchema";

import { AccountService } from "../../applications/services/AccountService";

import fastify from "../../server/fastify";

const accountService = new AccountService();

const options: RouteShorthandOptions  = {
    schema: {
        tags: ['account'],
        summary: 'Update values of account',
        querystring: { $ref: SCHEMA_QUERY_USERNAME_ACCOUNT },
        body: { $ref: SCHEMA_UPDATE_ACCOUNT },
        response: {
            200: { $ref: SCHEMA_RETURN_ACCOUNT },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
            404: { $ref: SCHEMA_NOT_FOUND },
        }
    }
}

fastify.put<{Querystring: staticQueryUsernameAccount, Body: staticUpdateAccount}>(`${PATH_BASE_CONTROLLER}/update`, options, async (request) => {
    const {username} = request.query;
    const account = request.body;

    console.log(request.body);
    
    return await accountService.updateAccount(username, account);
})