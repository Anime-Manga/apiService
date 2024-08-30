import { Repository } from "typeorm";

import { Account } from "../../../domain/models/Account";
import { IAccount } from "../../../domain/interfaces/models/IAccount";

export default interface IAccountRepository {
    connectionRepository: Repository<Account>;

    //get
    findFromUsername(username: string): Promise<Account>;
    //put
    createAccount(data: IAccount): Promise<Account>;
    //post
    updateAccount(username: string, data: Partial<IAccount>): Promise<void>;
    //delete
    deleteAccount(username: string): void;
}