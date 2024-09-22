import async from "async";

import { objectAssign } from "../../utils/genericUtils";

import FriendRepository from "../repositories/FriendRepository";
import IFriendService from "../interfaces/services/IFriendService";

import { ISchemaReturnPaginatedFriend } from "../../schemas/friendSchema";
import { IFriendDTO, IFriendDTOEnv } from "../../domain/interfaces/DTOs/IFriendDTO";

export class FriendService implements IFriendService {
    friendRepository = new FriendRepository();

    //get
    async listPaginated(username: string, skip: number, length: number): Promise<ISchemaReturnPaginatedFriend> {
        const {count, list} = await async.parallel<unknown, {list: Array<IFriendDTO>, count: number}>({
            list: async () => (await this.friendRepository.listPaginated(username, skip, length)).map((reqeust) => objectAssign<IFriendDTO>(reqeust, IFriendDTOEnv)),
           count: async () => await this.friendRepository.countTotalFromUsername(username)
        });

        return {
            list,
            max_count: count
        };
    }

    //delete
    async deleteFriend(username: string, friend: string): Promise<void> {
        this.friendRepository.deleteFriend(username, friend);
    }
}