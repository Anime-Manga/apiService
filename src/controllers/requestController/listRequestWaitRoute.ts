import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, schemaQueryPaginatedRequest, schemaReturnPaginatedRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

fastify.get(`${PATH_BASE_CONTROLLER}/list_request_wait`, {
    schema: {
        tags: ["request"],
        summary: "List your request",
        querystring: schemaQueryPaginatedRequest,
        response: {
            200: schemaReturnPaginatedRequest,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {length, auth_username, skip} = request.query;

    replay.status(200).send(await requestService.listRequestWaitPaginated(auth_username, skip, length));
});