import { FriendService } from "../../applications/services/FriendService";

import { PATH_BASE_CONTROLLER, schemaQueryUsernameOfFriend } from "../../schemas/friendSchema";
import { SCHEMA_INTERNAL_SERVER, schemaReturnVoid } from "../../schemas/generic/genericSchema";

import fastify from "../../server/fastify";

const friendService = new FriendService();

fastify.delete(`${PATH_BASE_CONTROLLER}/remove`, {
    schema: {
        tags: ["friend"],
        summary: "Remove my friend",
        querystring: schemaQueryUsernameOfFriend,
        response: {
            200: schemaReturnVoid,
            500: { $ref: SCHEMA_INTERNAL_SERVER }
        }
    }
}, async (request, replay) => {
    const {friend, auth_username} = request.query;

    await friendService.deleteFriend(auth_username, friend);

    replay.status(200).send({ response: "ok" });
});