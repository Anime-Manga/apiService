import { RouteShorthandOptions } from "fastify";

import fastify from "../../server/fastify";
import { RequestService } from "../../applications/services/RequestService";
import { PATH_BASE_CONTROLLER, SCHEMA_CREATE_REQUEST, staticCreateRequest } from "../../schemas/requestSchema";
import { SCHEMA_ERROR, SCHEMA_RETURN_VOID } from "../../schemas/generic/genericSchema";

const requestService = new RequestService();

const options: RouteShorthandOptions  = {
    schema: {
        body: { $ref: SCHEMA_CREATE_REQUEST },
        response: {
            200: { $ref: SCHEMA_RETURN_VOID },
            401: { $ref: SCHEMA_ERROR },
            404: { $ref: SCHEMA_ERROR },
        }
    }
}

fastify.post<{Body: staticCreateRequest}>(`${PATH_BASE_CONTROLLER}/create`, options, async (request) => {
    await requestService.create(request.body);

    return { response: "ok" }
})