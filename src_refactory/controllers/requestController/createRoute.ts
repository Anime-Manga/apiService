import { RouteShorthandOptions } from "fastify";

import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, SCHEMA_CREATE_REQUEST, staticCreateRequest } from "../../schemas/requestSchema";
import { SCHEMA_BAD_REQUEST, SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND, SCHEMA_RETURN_VOID } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

const options: RouteShorthandOptions = {
    schema: {
        tags: ["request"],
        summary: "Create request friend",
        body: { $ref: SCHEMA_CREATE_REQUEST },
        response: {
            200: { $ref: SCHEMA_RETURN_VOID },
            401: { $ref: SCHEMA_BAD_REQUEST },
            404: { $ref: SCHEMA_NOT_FOUND },
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
};

fastify.post<{Body: staticCreateRequest}>(`${PATH_BASE_CONTROLLER}/create`, options, async (request) => {
    await requestService.create(request.body);

    return { response: "ok" };
});