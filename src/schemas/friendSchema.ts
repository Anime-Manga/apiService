import { Type } from "@sinclair/typebox";

import { friendSchema, IFriendEnv } from "../domain/interfaces/models/IFriend";
import { friendDTOSchema } from "../domain/interfaces/DTOs/IFriendDTO";
import { queryPagination, schemaReturnPagination } from "./generic/paginationSchema";
import { requireAuth } from "./generic/authSchema";

//path base
const PATH_BASE_CONTROLLER = "/friend";

//request
const schemaQueryUsernameOfFriend = requireAuth(Type.Pick(friendSchema, [IFriendEnv.FRIEND]));
const schemaQueryPaginatedFriend = requireAuth(queryPagination);

//replay
const schemaReturnFriend = friendDTOSchema;
const schemaReturnPaginatedFriend = schemaReturnPagination(friendDTOSchema);

//interface
type ISchemaReturnPaginatedFriend = typeof schemaReturnPaginatedFriend.static;

export {
    PATH_BASE_CONTROLLER,

    //request
    schemaQueryUsernameOfFriend,
    schemaQueryPaginatedFriend,

    //replay
    schemaReturnFriend,
    schemaReturnPaginatedFriend,

    //interface
    ISchemaReturnPaginatedFriend
};