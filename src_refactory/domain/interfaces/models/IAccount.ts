export enum IAccountEnv {
    USERNAME = "username",
    PASSWORD = "password",
    LAST_ACCESS = "last_access",
    CHANGE_PASSWORD = "change_password",
    EXPIRE_PASSWORD = "expire_password"
}

export interface IAccount {
    [IAccountEnv.USERNAME]: string,
    [IAccountEnv.PASSWORD]: string,
    [IAccountEnv.LAST_ACCESS]: Date,
    [IAccountEnv.CHANGE_PASSWORD]: boolean,
    [IAccountEnv.EXPIRE_PASSWORD]: Date
}