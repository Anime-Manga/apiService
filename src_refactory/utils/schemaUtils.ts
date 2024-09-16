import { TSchema, Type } from "@sinclair/typebox";

function convertToOptional<T extends TSchema, K extends PropertyKey>(schema: T, keys: Array<K>, id: string){
    const pick = Type.Pick(schema, keys);

    return Type.Mapped(Type.KeyOf(pick), K => Type.Optional(Type.Index(pick, K)), { $id: id });
}

export {
    convertToOptional
}