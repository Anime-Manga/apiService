import IAccountService from "../interfaces/services/IAccountService";
import { IAccountDTO, IAccountDTOEnv } from "../../domain/interfaces/DTOs/IAccountDTO";
import { IAccount, IAccountEnv } from "../../domain/interfaces/models/IAccount";

import AccountRepository from "../repositories/AccountRepository";

import {hashPassword} from "../../utils/AccountUtils";
import { ApiConflict, ApiNotFound, ApiUnauthorized } from "../../modules/api";

import { objectAssign } from "../../utils/utils";
import { Account } from "../../domain/models/Account";

export class AccountService implements IAccountService {
    accountRepository = new AccountRepository();

    async login(username: string, password: string): Promise<IAccountDTO> {
        let user: Account;

        try{
            user = await this.accountRepository.findFromUsername(username);
        }catch(e){
            if(e instanceof ApiNotFound){
                throw new ApiUnauthorized("Username/Password wrong");
            }else{
                throw new e;
            }
        }

        if(hashPassword(password) !== user[IAccountEnv.PASSWORD]){
            throw new ApiUnauthorized("Username/Password wrong");
        }

        return objectAssign(user, IAccountDTOEnv);
    }
    async findFromUsername(username: string): Promise<IAccountDTO> {
        const user = await this.accountRepository.findFromUsername(username);

        return objectAssign(user, IAccountDTOEnv);
    }
    async createAccount(data: IAccount): Promise<IAccountDTO> {
        const username = data[IAccountEnv.USERNAME];

        try{
            await this.accountRepository.findFromUsername(username);
        }catch(e){
            if(e instanceof ApiNotFound){
                data[IAccountEnv.PASSWORD] = hashPassword(data[IAccountEnv.PASSWORD]);
                const createdAccount = await this.accountRepository.createAccount(data);

                return objectAssign(createdAccount, IAccountDTOEnv);
            }else{
                throw new e;
            }
        }

        throw new ApiConflict(`This username already used ${username}`);
    }
    updateAccount(data: IAccount): Promise<IAccountDTO> {
        throw new Error("Method not implemented.");
    }
    deleteAccount(username: string): void {
        throw new Error("Method not implemented.");
    }
}