export enum IAccountDTOEnv {
    USERNAME = "username",
    LAST_ACCESS = "last_access",
    CHANGE_PASSWORD = "change_password",
    EXPIRE_PASSWORD = "expire_password",
    PROFILE = "profile"
}

export interface IAccountDTO {
    [IAccountDTOEnv.USERNAME]: string,
    [IAccountDTOEnv.LAST_ACCESS]: Date,
    [IAccountDTOEnv.CHANGE_PASSWORD]: Date,
    [IAccountDTOEnv.EXPIRE_PASSWORD]: Date,
    [IAccountDTOEnv.PROFILE]: string
}