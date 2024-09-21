import { Type } from "@sinclair/typebox";

const SCHEMA_NOT_FOUND = "SchemaNotFound";
const SCHEMA_INTERNAL_SERVER = "SchemaInternalServer";
const SCHEMA_CONFLICT = "SchemaConflict";
const SCHEMA_BAD_REQUEST = "SchemaBadRequest";

const schemaReturnVoid = Type.Object({
    response: Type.String({default: "ok"})
});

const templateErrorSchema = Type.Object({
    message: Type.String({default: "Example error"}),
    error: Type.String({default: "Name error"})
});

const notFound = Type.Intersect([
templateErrorSchema, Type.Object({
    statusCode: Type.Number({default: 404})
})
], { $id: SCHEMA_NOT_FOUND });
const internalServer = Type.Intersect([
templateErrorSchema, Type.Object({
    statusCode: Type.Number({default: 500})
})
], { $id: SCHEMA_INTERNAL_SERVER });
const conflict = Type.Intersect([
templateErrorSchema, Type.Object({
    statusCode: Type.Number({default: 401})
})
], { $id: SCHEMA_CONFLICT });
const badRequest = Type.Intersect([
templateErrorSchema, Type.Object({
    statusCode: Type.Number({default: 400})
})
], { $id: SCHEMA_BAD_REQUEST });

export default [notFound, internalServer, conflict, badRequest];

export {
    //replay
    schemaReturnVoid,

    //replay with ID
    SCHEMA_NOT_FOUND,
    SCHEMA_BAD_REQUEST,
    SCHEMA_CONFLICT,
    SCHEMA_INTERNAL_SERVER
};