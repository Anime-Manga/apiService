import { Type } from "@sinclair/typebox";

import { accountSchema, IAccountEnv } from "../domain/interfaces/models/IAccount";
import { accountDTOSchema } from "../domain/interfaces/DTOs/IAccountDTO";
import { convertToOptional } from "../utils/schemaUtils";
import { requireAuth } from "./generic/authSchema";

//path base
const PATH_BASE_CONTROLLER = "/account";

//request
const schemaCreateAccount = Type.Pick(accountSchema, [IAccountEnv.USERNAME, IAccountEnv.PASSWORD, IAccountEnv.CHANGE_PASSWORD]);
const schemaUpdateAccount = convertToOptional(accountSchema, [IAccountEnv.PROFILE, IAccountEnv.CHANGE_PASSWORD]);
const schemaQueryUsernameAccount = requireAuth(Type.Pick(accountSchema, [IAccountEnv.USERNAME]));

//replay
const schemaReturnAccount = accountDTOSchema;

export {
    PATH_BASE_CONTROLLER,

    //request
    schemaCreateAccount,
    schemaUpdateAccount,
    schemaQueryUsernameAccount,

    //replay
    schemaReturnAccount
};