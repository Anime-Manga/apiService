import { Like } from "typeorm";

import {pg} from "../../server/postgres";
import { ApiErrorGeneric, ApiNotFound } from "../../modules/api";

import IAccountRepository from "../interfaces/repositories/IAccountRepository";
import { IAccount, IAccountEnv } from "../../domain/interfaces/models/IAccount";

import { Account } from "../../domain/models/Account";

import Logger from "../../modules/logger";
const logger = new Logger("account-db");

export default class AccountRepository implements IAccountRepository {
    connectionRepository = pg.getRepository(Account);

    async findFromUsername(username: string): Promise<Account> {
        let rs: Account | null = null;

        try{
            rs = await this.connectionRepository.findOneBy({ [IAccountEnv.USERNAME]: Like(username) });
        }catch(err){
            logger.error("Failed exec query findFromUsername, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if(rs === null){
            throw new ApiNotFound(`Not found this username '${username}'`);
        }

        return rs;
    }
    
    async createAccount(data: IAccount): Promise<Account> {
        try{
            await this.connectionRepository.insert(data)
        }catch(err){
            logger.error("Failed exec query createAccount, details:", err);
            throw new ApiErrorGeneric(err);
        }

        return data; 
    }
    updateAccount(data: IAccount): Promise<Account> {
        throw new Error("Method not implemented.");
    }
    deleteAccount(username: string): void {
        throw new Error("Method not implemented.");
    }
    
}