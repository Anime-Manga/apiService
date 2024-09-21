import { RouteShorthandOptions } from "fastify";

import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, schemaQueryPaginatedRequest, schemaReturnPaginatedRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

fastify.get(`${PATH_BASE_CONTROLLER}/list`, {
    schema: {
        tags: ["request"],
        summary: "List request for accept or reject",
        querystring: schemaQueryPaginatedRequest,
        response: {
            200: schemaReturnPaginatedRequest,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {length, request_from, skip} = request.query;

    replay.status(200).send(await requestService.listPaginated(request_from, skip, length));
});