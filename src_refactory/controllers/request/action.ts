import { RouteShorthandOptions } from "fastify";

import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, SCHEMA_ACTION_REQUEST, staticActionRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER, SCHEMA_RETURN_VOID } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

const options: RouteShorthandOptions  = {
    schema: {
        tags: ['request'],
        summary: 'Choose accept or refuse request friend',
        querystring: { $ref: SCHEMA_ACTION_REQUEST },
        response: {
            200: { $ref: SCHEMA_RETURN_VOID },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
        }
    }
}

fastify.post<{Querystring: staticActionRequest}>(`${PATH_BASE_CONTROLLER}/action`, options, async (request) => {
    const {accept, request_from, request_to} = request.query;

    await requestService.action(request_from, request_to, accept);

    return { response: "ok" }
})