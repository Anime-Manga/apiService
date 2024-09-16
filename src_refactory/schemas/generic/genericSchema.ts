import { Type } from '@sinclair/typebox'

const SCHEMA_RETURN_VOID = "SchemaReturnVoid";
const SCHEMA_ERROR = "SchemaError";

const returnVoid = Type.Object({
    response: Type.String()
}, {$id: SCHEMA_RETURN_VOID});

const errorSchema = Type.Object({
    statusCode: Type.Number(),
    message: Type.String(),
    error: Type.String()
}, { $id: SCHEMA_ERROR })

export default [returnVoid, errorSchema]

type staticReturnVoid = typeof returnVoid.static

export {
    staticReturnVoid,

    SCHEMA_RETURN_VOID,
    SCHEMA_ERROR
}