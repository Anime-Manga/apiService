import { Account } from "../models/accounts.model.js";

export type AccountDTO = Pick<Account, "createdAt" | "updatedAt" | "role" | "username">;

export function AccountToAccountDTO(account: Account): AccountDTO {
    return {
        createdAt: account.createdAt,
        role: account.role,
        updatedAt: account.updatedAt,
        username: account.username
    }
}