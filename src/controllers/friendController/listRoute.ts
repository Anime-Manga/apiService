import { FriendService } from "../../applications/services/FriendService";

import { PATH_BASE_CONTROLLER, schemaQueryPaginatedFriend, schemaReturnPaginatedFriend } from "../../schemas/friendSchema";
import { SCHEMA_INTERNAL_SERVER } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const friendService = new FriendService();

fastify.get(`${PATH_BASE_CONTROLLER}/list`, {
    schema: {
        tags: ["friend"],
        summary: "List my friends",
        querystring: schemaQueryPaginatedFriend,
        response: {
            200: schemaReturnPaginatedFriend,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {length, skip, auth_username} = request.query;

    replay.status(200).send(await friendService.listPaginated(auth_username, skip, length));
});