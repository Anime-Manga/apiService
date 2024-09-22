import { ISchemaReturnPaginatedFriend } from "../../../schemas/friendSchema";

export default interface IFriendService {
    //get
    listPaginated(username: string, skip: number, length: number): Promise<ISchemaReturnPaginatedFriend>;

    //delete
    deleteFriend(username: string, friend: string): Promise<void>;
} // eslint-disable-line @stylistic/semi