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
    [IAccountEnv.LAST_ACCESS]: Type.Union([Type.String({ format: "date-time" }), Type.Null()]),
    [IAccountEnv.PROFILE]: Type.Union([Type.String(), Type.Null()]),
    [IAccountEnv.CHANGE_PASSWORD]: Type.Boolean({ default: false })
});

export type IAccount = typeof accountSchema.static