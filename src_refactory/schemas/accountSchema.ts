import { Type } from '@sinclair/typebox'

import { accountSchema, IAccountEnv } from "../domain/interfaces/models/IAccount"
import { accountDTOSchema } from '../domain/interfaces/DTOs/IAccountDTO';
import { convertToOptional } from '../utils/schemaUtils';

//path base
const PATH_BASE_CONTROLLER = "/account";

//schemas
const SCHEMA_CREATE_ACCOUNT = "SchemaCreateAccount";
const SCHEMA_UPDATE_ACCOUNT = "SchemaUpdateAccount";
const SCHEMA_RETURN_ACCOUNT = "SchemaReturnAccount";
const SCHEMA_QUERY_USERNAME_ACCOUNT = "SchemaQueryUsernameAccount";


//request
const createAccount = Type.Pick(accountSchema, [IAccountEnv.USERNAME, IAccountEnv.PASSWORD, IAccountEnv.CHANGE_PASSWORD], { $id: SCHEMA_CREATE_ACCOUNT });
const updateAccount = convertToOptional(accountSchema, [IAccountEnv.PROFILE, IAccountEnv.CHANGE_PASSWORD], SCHEMA_UPDATE_ACCOUNT);
const queryUsernameAccount = Type.Pick(accountSchema, [IAccountEnv.USERNAME], { $id: SCHEMA_QUERY_USERNAME_ACCOUNT });

//replay
const returnAccount = Type.Intersect([accountDTOSchema], { $id: SCHEMA_RETURN_ACCOUNT });

//load schemas
export default [createAccount, updateAccount, returnAccount, queryUsernameAccount]

type staticCreateAccount = typeof createAccount.static
type staticUpdateAccount = typeof updateAccount.static
type staticReturnAccount = typeof returnAccount.static
type staticQueryUsernameAccount = typeof queryUsernameAccount.static;

export {
    PATH_BASE_CONTROLLER,

    staticCreateAccount,
    staticUpdateAccount,
    staticReturnAccount,
    staticQueryUsernameAccount,

    SCHEMA_CREATE_ACCOUNT,
    SCHEMA_RETURN_ACCOUNT,
    SCHEMA_UPDATE_ACCOUNT,
    SCHEMA_QUERY_USERNAME_ACCOUNT
}