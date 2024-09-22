import { Type, TObject } from "@sinclair/typebox";

const queryPagination = Type.Object({
    skip: Type.Number({minimum: 0, default: 0}),
    length: Type.Number({minimum: 10, default: 10})
});

function schemaReturnPagination<T extends TObject>(schema: T){
    return Type.Object({
        max_count: Type.Number(),
        list: Type.Array(schema)
    });
}

export {
    schemaReturnPagination,

    queryPagination
};