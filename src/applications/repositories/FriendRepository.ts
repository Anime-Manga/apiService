import { DeleteResult } from "typeorm";

import {pg} from "../../server/postgres";
import { ApiErrorGeneric, ApiNotFound } from "../../modules/api";

import IFriendRepository from "../interfaces/repositories/IFriendRepository";

import { IFriend, IFriendEnv } from "../../domain/interfaces/models/IFriend";
import { Friend } from "../../domain/models/Friends";

import Logger from "../../modules/logger";
const logger = new Logger("friend-db");

export default class FriendRepository implements IFriendRepository {
    connectionRepository = pg.getRepository<IFriend>(Friend);

    //get
    async findByFriend(username: string, friend: string): Promise<IFriend> {
        let rs: IFriend | null = null;

        try {
            rs = await this.connectionRepository.findOneBy({[IFriendEnv.USERNAME]: username, [IFriendEnv.FRIEND]: friend });
        } catch (err){
            logger.error("Failed exec query findByFriend, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs === null){
            throw new ApiNotFound(`Not found friend: ${friend}`);
        }

        return rs;
    }
    async listPaginated(username: string, skip: number, length: number): Promise<Array<IFriend>> {
        let rs: Array<IFriend> = [];

        try {
            rs = await this.connectionRepository.find({ where: { [IFriendEnv.USERNAME]: username }, order: { [IFriendEnv.BECOME_FRIEND_TIME]: "DESC" }, skip, take: length });
        } catch (err){
            logger.error("Failed exec query listPaginated, details:", err);
            throw new ApiErrorGeneric(err);
        }

        return rs;
    }
    async countTotalFromUsername(username: string): Promise<number> {
        try {
            return await this.connectionRepository.countBy({ [IFriendEnv.USERNAME]: username });
        } catch (err){
            logger.error("Failed exec query countTotal, details:", err);
            throw new ApiErrorGeneric(err);
        }
    }

    //post
    async createFriend(data: Partial<IFriend>): Promise<void> {
        try {
            await this.connectionRepository.insert(data);
        } catch (err){
            logger.error("Failed exec query createFriend, details:", err);
            throw new ApiErrorGeneric(err);
        }
    }

    //delete
    async deleteFriend(username: string, friend: string): Promise<void> {
        let rs: DeleteResult;
        try {
            rs = await this.connectionRepository.delete({ [IFriendEnv.FRIEND]: friend, [IFriendEnv.USERNAME]: username });
        } catch (err){
            logger.error("Failed exec query deleteFriend, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs.affected <= 0){
            throw new ApiNotFound(`Cannot delete friend ${friend}`);
        }
    }
}