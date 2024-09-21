import { Type } from "@sinclair/typebox";
import { queryPagination, schemaReturnPagination } from "./generic/paginationSchema";
import { requestDTOSchema } from "../domain/interfaces/DTOs/IRequestDTO";
import { IRequestEnv, requestSchema } from "../domain/interfaces/models/IRequest";

//path base
const PATH_BASE_CONTROLLER = "/request";

//schemas
const SCHEMA_CREATE_REQUEST = "SchemaCreateRequest";
const SCHEMA_ACTION_REQUEST = "SchemaActionRequest";
const SCHEMA_RETURN_PAGINATED_REQUEST = "SchemaReturnPaginatedRequest";
const SCHEMA_QUERY_PAGINATED_REQUEST = "SchemaQueryPaginatedRequest";

//request
const createRequest = Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM, IRequestEnv.REQUEST_TO], { $id: SCHEMA_CREATE_REQUEST });
const actionRequest = Type.Intersect([
    Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM, IRequestEnv.REQUEST_TO]),
    Type.Object({
        accept: Type.Boolean()
    })
], {$id: SCHEMA_ACTION_REQUEST});
const queryPaginatedRequest = Type.Intersect([Type.Pick(requestSchema, [IRequestEnv.REQUEST_FROM]), queryPagination], {$id: SCHEMA_QUERY_PAGINATED_REQUEST});

//replay
const returnPaginatedRequest = schemaReturnPagination(requestDTOSchema, SCHEMA_RETURN_PAGINATED_REQUEST);

//load schemas
export default [createRequest, actionRequest, returnPaginatedRequest, queryPaginatedRequest];

type staticCreateRequest = typeof createRequest.static;
type staticActionRequest = typeof actionRequest.static;
type staticReturnPaginatedRequest = typeof returnPaginatedRequest.static;
type staticQueryPaginatedRequest = typeof queryPaginatedRequest.static;

export {
    PATH_BASE_CONTROLLER,
    
    staticCreateRequest,
    staticActionRequest,
    staticReturnPaginatedRequest,
    staticQueryPaginatedRequest,

    SCHEMA_CREATE_REQUEST,
    SCHEMA_ACTION_REQUEST,
    SCHEMA_RETURN_PAGINATED_REQUEST,
    SCHEMA_QUERY_PAGINATED_REQUEST
};