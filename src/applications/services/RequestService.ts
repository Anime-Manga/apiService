import async from "async";

import { ApiBadRequest, ApiConflict, ApiNotFound } from "../../modules/api";

import { objectAssign } from "../../utils/genericUtils";

import { IRequest, IRequestEnv } from "../../domain/interfaces/models/IRequest";
import { IRequestDTO, IRequestDTOEnv } from "../../domain/interfaces/DTOs/IRequestDTO";

import { ISchemaReturnPaginatedRequest } from "../../schemas/requestSchema";

import IRequestService from "../interfaces/services/IRequestService";

import FriendRepository from "../repositories/FriendRepository";
import RequestRepository from "../repositories/RequestRepository";
import AccountRepository from "../repositories/AccountRepository";
import { IFriend, IFriendEnv } from "../../domain/interfaces/models/IFriend";

export class RequestService implements IRequestService {
    requestRepository = new RequestRepository();
    accountRepository = new AccountRepository();
    friendRepository = new FriendRepository();

    //get
    async listRequestWaitPaginated(request_from: string, skip: number, length: number): Promise<ISchemaReturnPaginatedRequest> {
        const {count, list} = await async.parallel<unknown, {list: Array<IRequestDTO>, count: number}>({
            list: async () => (await this.requestRepository.listPaginatedFromRequestFrom(request_from, skip, length)).map((reqeust) => objectAssign<IRequestDTO>(reqeust, IRequestDTOEnv)),
           count: async () => await this.requestRepository.countTotalFromRequestFrom(request_from)
        });

        return {
            list,
            max_count: count
        };
    }
    async listRequestForActionPaginated(request_to: string, skip: number, length: number): Promise<ISchemaReturnPaginatedRequest> {
        const {count, list} = await async.parallel<unknown, {list: Array<IRequestDTO>, count: number}>({
            list: async () => (await this.requestRepository.listPaginatedFromRequestTo(request_to, skip, length)).map((reqeust) => objectAssign<IRequestDTO>(reqeust, IRequestDTOEnv)),
           count: async () => await this.requestRepository.countTotalFromRequestFrom(request_to)
        });

        return {
            list,
            max_count: count
        };
    }

    //post
    async create(auth_username: string, request_to: string): Promise<void> {
        request_to = request_to.toLowerCase();

        if (auth_username === request_to){
            throw new ApiBadRequest(`${IRequestEnv.REQUEST_FROM} cannot be same ${IRequestEnv.REQUEST_TO}`);
        }

        const {findRequest, findRequestTO, alreadyFriend} = await async.parallel<unknown, {findRequest: boolean, findRequestTO: boolean, alreadyFriend: boolean}>({
            findRequest: async () => {
                try {
                    await this.requestRepository.findByRequest(auth_username, request_to);
                } catch (err){
                    if (err instanceof ApiNotFound){
                        return false;
                    } else {
                        throw new err; 
                    }
                }

                return true;
            },
            findRequestTO: async () => {
                try {
                    await this.accountRepository.findFromUsername(request_to);
                } catch (err){
                    if (err instanceof ApiNotFound){
                        return false;
                    } else {
                        throw new err; 
                    }
                }

                return true;
            },
            alreadyFriend: async () => {
                try {
                    await this.friendRepository.findByFriend(auth_username, request_to);
                } catch (err){
                    if (err instanceof ApiNotFound){
                        return false;
                    } else {
                        throw new err; 
                    }
                }

                return true; 
            }
        });

        if (!findRequest && findRequestTO && !alreadyFriend){
            const data: Partial<IRequest> = {
                [IRequestEnv.REQUEST_FROM]: auth_username,
                [IRequestEnv.REQUEST_TO]: request_to
            };
            return await this.requestRepository.create(data);
        } else if (!findRequestTO){
            throw new ApiNotFound(`The user "${request_to}" not exist!`);
        } else if (alreadyFriend){
            throw new ApiConflict("He is already your friend");
        } else {
            throw new ApiConflict("The request to become a friend has already been sent");
        }
    }
    async action(request_from: string, auth_username: string, accept: boolean): Promise<void> {
        await this.requestRepository.findByRequest(request_from, auth_username);
        
        if (accept){
            const data: Partial<IFriend> = {
                [IFriendEnv.USERNAME]: request_from,
                [IFriendEnv.FRIEND]: auth_username
            };

            await this.friendRepository.createFriend(data);
        }
        
        await this.requestRepository.delete(request_from, auth_username);
    }
    async delete(auth_username: string, request_to: string): Promise<void> {
        await this.requestRepository.delete(auth_username, request_to);
    }
}