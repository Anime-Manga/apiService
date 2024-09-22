import _ from "lodash";

import IAccountService from "../interfaces/services/IAccountService";
import { IAccountDTO, IAccountDTOEnv } from "../../domain/interfaces/DTOs/IAccountDTO";
import { IAccount, IAccountEnv } from "../../domain/interfaces/models/IAccount";

import AccountRepository from "../repositories/AccountRepository";

import {hashPassword} from "../../utils/accountUtils";
import { ApiConflict, ApiNotFound, ApiUnauthorized } from "../../modules/api";

import { objectAssign } from "../../utils/genericUtils";

export class AccountService implements IAccountService {
    accountRepository = new AccountRepository();

    //get
    async findFromUsername(username: string): Promise<IAccountDTO> {
        const user = await this.accountRepository.findFromUsername(username);

        return objectAssign<IAccountDTO>(user, IAccountDTOEnv);
    }

    //post
    async login(username: string, password: string): Promise<IAccountDTO> {
        let user: IAccount;

        try {
            user = await this.accountRepository.findFromUsername(username);
        } catch (e){
            if (e instanceof ApiNotFound){
                throw new ApiUnauthorized("Username/Password wrong");
            } else {
                throw new e;
            }
        }

        if (hashPassword(password) !== user[IAccountEnv.PASSWORD]){
            throw new ApiUnauthorized("Username/Password wrong");
        }

        return objectAssign<IAccountDTO>(user, IAccountDTOEnv);
    }
    async createAccount(data: Partial<IAccount>): Promise<IAccountDTO> {
        const username = data[IAccountEnv.USERNAME];

        try {
            await this.accountRepository.findFromUsername(username);
        } catch (e){
            if (e instanceof ApiNotFound){
                data[IAccountEnv.PASSWORD] = hashPassword(data[IAccountEnv.PASSWORD]);
                await this.accountRepository.createAccount(data);

                const user = await this.accountRepository.findFromUsername(data[IAccountEnv.USERNAME]);
                return objectAssign<IAccountDTO>(user, IAccountDTOEnv);
            } else {
                throw new e;
            }
        }

        throw new ApiConflict(`This username already used ${username}`);
    }

    //update
    async updateAccount(username: string, data: Partial<IAccount>): Promise<IAccountDTO> {
        const user = await this.accountRepository.findFromUsername(username);

        await this.accountRepository.updateAccount(username, data);

        //update information
        const newUser = _.merge(user, data);

        return objectAssign<IAccountDTO>(newUser, IAccountDTOEnv);
    }

    //delete
    async deleteAccount(username: string): Promise<void> {
        await this.accountRepository.deleteAccount(username);
    }
}