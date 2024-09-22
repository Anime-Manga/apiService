import { Type } from "@sinclair/typebox";
import { queryPagination, schemaReturnPagination } from "./generic/paginationSchema";
import { requestDTOSchema } from "../domain/interfaces/DTOs/IRequestDTO";
import { IRequestEnv, requestSchema } from "../domain/interfaces/models/IRequest";
import { schemaQueryUsernameAuth, requireAuth } from "./generic/authSchema";

//path base
const PATH_BASE_CONTROLLER = "/request";

//request
const schemaCreateRequest = Type.Pick(requestSchema, [IRequestEnv.REQUEST_TO]);
const schemaActionRequest = Type.Intersect([
    schemaQueryUsernameAuth,
    Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM]),
    Type.Object({
        accept: Type.Boolean()
    })
]);
const schemaQueryPaginatedRequest = requireAuth(queryPagination);
const schemaQueryDeleteRequest = requireAuth(Type.Pick(requestSchema, [IRequestEnv.REQUEST_TO]));

//replay
const schemaReturnPaginatedRequest = schemaReturnPagination(requestDTOSchema);

//interface
type ISchemaReturnPaginatedRequest = typeof schemaReturnPaginatedRequest.static;

export {
    PATH_BASE_CONTROLLER,
    
    //request
    schemaCreateRequest,
    schemaActionRequest,
    schemaQueryPaginatedRequest,
    schemaQueryDeleteRequest,

    //replay
    schemaReturnPaginatedRequest,

    //interfaces
    ISchemaReturnPaginatedRequest
};