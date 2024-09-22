import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, schemaQueryDeleteRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER, schemaReturnVoid } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

fastify.delete(`${PATH_BASE_CONTROLLER}/remove`, {
    schema: {
        tags: ["request"],
        summary: "Remove request",
        querystring: schemaQueryDeleteRequest,
        response: {
            200: schemaReturnVoid,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {request_to, auth_username} = request.query;

    await requestService.delete(auth_username, request_to);

    replay.status(200).send({ response: "ok" });
});