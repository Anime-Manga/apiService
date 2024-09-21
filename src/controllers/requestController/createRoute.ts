import { RequestService } from "../../applications/services/RequestService";

import { PATH_BASE_CONTROLLER, schemaCreateRequest } from "../../schemas/requestSchema";
import { SCHEMA_BAD_REQUEST, SCHEMA_INTERNAL_SERVER, SCHEMA_NOT_FOUND, schemaReturnVoid } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const requestService = new RequestService();

fastify.post(`${PATH_BASE_CONTROLLER}/create`, {
    schema: {
        tags: ["request"],
        summary: "Create request friend",
        body: schemaCreateRequest,
        response: {
            200: schemaReturnVoid,
            401: { $ref: SCHEMA_BAD_REQUEST },
            404: { $ref: SCHEMA_NOT_FOUND },
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    await requestService.create(request.body);

    replay.status(200).send({ response: "ok" });
});