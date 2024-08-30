export enum IAccountEnv {
    USERNAME = "username",
    PASSWORD = "password",
    LAST_ACCESS = "last_access",
    CHANGE_PASSWORD = "change_password",
    EXPIRE_PASSWORD = "expire_password",
    PROFILE = "profile"
}

export interface IAccount {
    [IAccountEnv.USERNAME]: string,
    [IAccountEnv.PASSWORD]: string,
    [IAccountEnv.LAST_ACCESS]: Date | null,
    [IAccountEnv.CHANGE_PASSWORD]: boolean,
    [IAccountEnv.EXPIRE_PASSWORD]: Date,
    [IAccountEnv.PROFILE]: string
}