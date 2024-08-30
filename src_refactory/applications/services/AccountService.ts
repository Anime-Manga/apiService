import _ from "lodash";

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

    //get
    async findFromUsername(username: string): Promise<IAccountDTO> {
        const user = await this.accountRepository.findFromUsername(username);

        return objectAssign(user, IAccountDTOEnv);
    }

    //post
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

    //update
    async updateAccount(username: string, data: IAccount): Promise<IAccountDTO> {
        const user = await this.accountRepository.findFromUsername(username);

        await this.accountRepository.updateAccount(username, data);

        //update information
        const newUser = _.merge(user, data);

        return objectAssign(newUser, IAccountDTOEnv);
    }

    //delete
    async deleteAccount(username: string): Promise<void> {
        await this.accountRepository.deleteAccount(username);
    }
}