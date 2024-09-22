import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, schemaActionRequest } from "../../schemas/requestSchema";
import { SCHEMA_INTERNAL_SERVER, schemaReturnVoid } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

fastify.post(`${PATH_BASE_CONTROLLER}/action`, {
    schema: {
        tags: ["request"],
        summary: "Choose accept or refuse request friend",
        querystring: schemaActionRequest,
        response: {
            200: schemaReturnVoid,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {accept, request_from, auth_username} = request.query;

    await requestService.action(request_from, auth_username, accept);

    replay.status(200).send({ response: "ok" });
});