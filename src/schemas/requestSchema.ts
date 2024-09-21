import { Type } from "@sinclair/typebox";
import { queryPagination, schemaReturnPagination } from "./generic/paginationSchema";
import { requestDTOSchema } from "../domain/interfaces/DTOs/IRequestDTO";
import { IRequestEnv, requestSchema } from "../domain/interfaces/models/IRequest";

//path base
const PATH_BASE_CONTROLLER = "/request";

//request
const schemaCreateRequest = Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM, IRequestEnv.REQUEST_TO]);
const schemaActionRequest = Type.Intersect([
    Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM, IRequestEnv.REQUEST_TO]),
    Type.Object({
        accept: Type.Boolean()
    })
]);
const schemaQueryPaginatedRequest = Type.Intersect([Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM]), queryPagination]);

//replay
const schemaReturnPaginatedRequest = schemaReturnPagination(requestDTOSchema);

export {
    PATH_BASE_CONTROLLER,
    
    //request
    schemaCreateRequest,
    schemaActionRequest,
    schemaQueryPaginatedRequest,

    //replay
    schemaReturnPaginatedRequest
};