import { RouteShorthandOptions } from "fastify";

import fastify from "../../server/fastify";
import { RequestService } from "../../applications/services/RequestService";
import { PATH_BASE_CONTROLLER, SCHEMA_QUERY_PAGINATED_REQUEST, SCHEMA_RETURN_PAGINATED_REQUEST, staticQueryPaginatedRequest } from "../../schemas/requestSchema";
import { SCHEMA_ERROR } from "../../schemas/generic/genericSchema";

const requestService = new RequestService();

const options: RouteShorthandOptions  = {
    schema: {
        querystring: { $ref: SCHEMA_QUERY_PAGINATED_REQUEST },
        response: {
            200: { $ref: SCHEMA_RETURN_PAGINATED_REQUEST },
            500: { $ref: SCHEMA_ERROR },
        }
    }
}

fastify.get<{Querystring: staticQueryPaginatedRequest}>(`${PATH_BASE_CONTROLLER}/list`, options, async (request) => {
    const {length, request_from, skip} = request.query;

    return await requestService.listPaginated(request_from, skip, length);
})