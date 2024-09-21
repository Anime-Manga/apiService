import { ApiBadRequest, ApiConflict, ApiNotFound } from "../../modules/api";

import { objectAssign } from "../../utils/genericUtils";

import { IRequestDTO, IRequestDTOEnv } from "../../domain/interfaces/DTOs/IRequestDTO";
import { IRequest, IRequestEnv } from "../../domain/interfaces/models/IRequest";

import IRequestService from "../interfaces/services/IRequestService";
import RequestRepository from "../repositories/RequestRepository";
import async from "async";
import { staticReturnPaginatedRequest } from "../../schemas/requestSchema";
import AccountRepository from "../repositories/AccountRepository";

export class RequestService implements IRequestService {
    requestRepository = new RequestRepository();
    accountRepository = new AccountRepository();

    async create(data: Partial<IRequest>): Promise<void> {
        if (data[IRequestEnv.REQUEST_FROM].toLowerCase() === data[IRequestEnv.REQUEST_TO].toLowerCase()){
            throw new ApiBadRequest(`${IRequestEnv.REQUEST_FROM} cannot be same ${IRequestEnv.REQUEST_TO}`);
        }

        const {findRequest, findRequestTO} = await async.parallel<unknown, {findRequest: boolean, findRequestTO: boolean}>({
            findRequest: async () => {
                try {
                    await this.requestRepository.findByRequest(data[IRequestEnv.REQUEST_FROM], data[IRequestEnv.REQUEST_TO]);
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
                    await this.accountRepository.findFromUsername(data[IRequestEnv.REQUEST_TO]);
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

        if (!findRequest && findRequestTO){
            return await this.requestRepository.create(data);
        } else if (!findRequestTO){
            throw new ApiNotFound(`The user "${data[IRequestEnv.REQUEST_TO]}" not exist!`);
        } else {
            throw new ApiConflict("The request to become a friend has already been sent");
        }
    }
    action(request_from: string, request_to: string, accept: boolean): Promise<void> {
        //todo table friends
        throw new Error("Method not implemented.");
    }
    async listPaginated(request_from: string, skip: number, length: number): Promise<staticReturnPaginatedRequest> {
        const {count, list} = await async.parallel<unknown, {list: Array<IRequestDTO>, count: number}>({
            list: async () => (await this.requestRepository.listPaginated(request_from, skip, length)).map((reqeust) => objectAssign<IRequestDTO>(reqeust, IRequestDTOEnv)),
           count: async () => await this.requestRepository.countTotalFromRequestFrom(request_from)
        });

        return {
            list,
            max_count: count
        };
    }
}