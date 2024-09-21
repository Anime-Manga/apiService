import { DeleteResult, UpdateResult } from "typeorm";

import {pg} from "../../server/postgres";
import { ApiErrorGeneric, ApiNotFound } from "../../modules/api";

import IAccountRepository from "../interfaces/repositories/IAccountRepository";
import { IAccount, IAccountEnv } from "../../domain/interfaces/models/IAccount";

import { Account } from "../../domain/models/Account";

import Logger from "../../modules/logger";
const logger = new Logger("account-db");

export default class AccountRepository implements IAccountRepository {
    connectionRepository = pg.getRepository<IAccount>(Account);

    //get
    async findFromUsername(username: string): Promise<IAccount> {
        let rs: IAccount | null = null;

        try {
            rs = await this.connectionRepository.findOneBy({ [IAccountEnv.USERNAME]: username });
        } catch (err){
            logger.error("Failed exec query findFromUsername, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs === null){
            throw new ApiNotFound(`Not found this username '${username}'`);
        }

        return rs;
    }

    //post
    async createAccount(data: Partial<IAccount>): Promise<void> {
        data[IAccountEnv.USERNAME] = data[IAccountEnv.USERNAME].toLowerCase();

        try {
            await this.connectionRepository.insert(data);
        } catch (err){
            logger.error("Failed exec query createAccount, details:", err);
            throw new ApiErrorGeneric(err);
        }
    }

    //put
    async updateAccount(username: string, data: Partial<IAccount>): Promise<void> {
        let rs: UpdateResult;
        try {
            rs = await this.connectionRepository.update({ [IAccountEnv.USERNAME]: username }, data);
        } catch (err){
            logger.error("Failed exec query updateAccount, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs.affected <= 0){
            throw new ApiNotFound(`Cannot update information of ${username}`);
        }
    }

    //delete
    async deleteAccount(username: string): Promise<void> {
        let rs: DeleteResult;
        try {
            rs = await this.connectionRepository.delete({ [IAccountEnv.USERNAME]: username });
        } catch (err){
            logger.error("Failed exec query updateAccount, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs.affected <= 0){
            throw new ApiNotFound(`Cannot delete ${username}`);
        }
    }
    
}