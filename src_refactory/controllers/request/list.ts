import { RouteShorthandOptions } from "fastify";

import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, SCHEMA_QUERY_PAGINATED_REQUEST, SCHEMA_RETURN_PAGINATED_REQUEST, staticQueryPaginatedRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

const options: RouteShorthandOptions  = {
    schema: {
        tags: ['request'],
        summary: 'List request for accept or reject',
        querystring: { $ref: SCHEMA_QUERY_PAGINATED_REQUEST },
        response: {
            200: { $ref: SCHEMA_RETURN_PAGINATED_REQUEST },
            500: { $ref: SCHEMA_INTERNAL_SERVER },
        }
    }
}

fastify.get<{Querystring: staticQueryPaginatedRequest}>(`${PATH_BASE_CONTROLLER}/list`, options, async (request) => {
    const {length, request_from, skip} = request.query;

    return await requestService.listPaginated(request_from, skip, length);
})