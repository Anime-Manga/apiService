import { Account } from "../../../domain/models/Account";
import { IAccount } from "../../../domain/interfaces/models/IAccount";
import IConnectionRepository from "./generic/IConnectionRepository";

export default interface IAccountRepository extends IConnectionRepository<IAccount>{
    //get
    findFromUsername(username: string): Promise<IAccount>;
    //put
    createAccount(data: IAccount): Promise<void>;
    //post
    updateAccount(username: string, data: Partial<IAccount>): Promise<void>;
    //delete
    deleteAccount(username: string): void;
}