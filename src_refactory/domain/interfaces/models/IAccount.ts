import { Type } from "@sinclair/typebox";

export enum IAccountEnv {
    USERNAME = "username",
    PASSWORD = "password",
    LAST_ACCESS = "last_access",
    CHANGE_PASSWORD = "change_password",
    EXPIRE_PASSWORD = "expire_password",
    PROFILE = "profile"
}

export const accountSchema = Type.Object({
    [IAccountEnv.USERNAME]: Type.String({ maxLength: 250 }),
    [IAccountEnv.PASSWORD]: Type.String({ maxLength: 250 }),
    [IAccountEnv.EXPIRE_PASSWORD]: Type.String({ format: "date-time" }),
    [IAccountEnv.LAST_ACCESS]: Type.Union([Type.Null(), Type.String({ format: "date-time" })]),
    [IAccountEnv.PROFILE]: Type.Union([Type.Null(), Type.String()]),
    [IAccountEnv.CHANGE_PASSWORD]: Type.Boolean()
});

export type IAccount = typeof accountSchema.static