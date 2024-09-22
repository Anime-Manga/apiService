import { IFriend } from "../../../domain/interfaces/models/IFriend";
import IConnectionRepository from "./generic/IConnectionRepository";

export default interface IFriendRepository extends IConnectionRepository<IFriend>{
    //get
    findByFriend(username: string, friend: string): Promise<IFriend>;
    listPaginated(username: string, skip: number, length: number): Promise<Array<IFriend>>;
    countTotalFromUsername(username: string): Promise<number>;

    //put
    createFriend(data: Partial<IFriend>): Promise<void>;
    
    //delete
    deleteFriend(username: string, friend: string): Promise<void>;
} // eslint-disable-line @stylistic/semi