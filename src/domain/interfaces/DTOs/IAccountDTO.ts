import { Type } from "@sinclair/typebox";

export enum IAccountDTOEnv {
    USERNAME = "username",
    LAST_ACCESS = "last_access",
    CHANGE_PASSWORD = "change_password",
    EXPIRE_PASSWORD = "expire_password",
    PROFILE = "profile"
}

export const accountDTOSchema = Type.Object({
    [IAccountDTOEnv.USERNAME]: Type.String({ maxLength: 250 }),
    [IAccountDTOEnv.EXPIRE_PASSWORD]: Type.String({ format: "date-time" }),
    [IAccountDTOEnv.LAST_ACCESS]: Type.Union([Type.Null(), Type.String({ format: "date-time" })]),
    [IAccountDTOEnv.PROFILE]: Type.Union([Type.Null(), Type.String()]),
    [IAccountDTOEnv.CHANGE_PASSWORD]: Type.Boolean({ default: false })
});

export type IAccountDTO = typeof accountDTOSchema.static