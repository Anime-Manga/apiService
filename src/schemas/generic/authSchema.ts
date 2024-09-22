import { Type, TObject } from "@sinclair/typebox";

const schemaQueryUsernameAuth = Type.Object({
    auth_username: Type.String({maxLength: 250})
});

function requireAuth<T extends TObject>(query: T){
    return Type.Intersect([schemaQueryUsernameAuth, query]);
}

export {
    schemaQueryUsernameAuth,
    
    requireAuth
};