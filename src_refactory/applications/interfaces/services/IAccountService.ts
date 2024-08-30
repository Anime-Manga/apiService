import {IAccountDTO} from "../../../domain/interfaces/DTOs/IAccountDTO"
import {IAccount} from "../../../domain/interfaces/models/IAccount"

export default interface IAccountService {
    //get
    findFromUsername(username: string): Promise<IAccountDTO>;
    login(username: string, password: string): Promise<IAccountDTO>;
    //put
    createAccount(data: IAccount): Promise<IAccountDTO>;
    //post
    updateAccount(username: string, data: Partial<IAccount>): Promise<IAccountDTO>;
    //delete
    deleteAccount(username: string): void;
}