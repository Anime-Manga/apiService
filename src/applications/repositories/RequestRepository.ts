import { DeleteResult } from "typeorm";

import {pg} from "../../server/postgres";
import { ApiErrorGeneric, ApiNotFound } from "../../modules/api";

import IRequestRepository from "../interfaces/repositories/IRequestRepository";
import { IRequest, IRequestEnv } from "../../domain/interfaces/models/IRequest";

import { Request } from "../../domain/models/Request";

import Logger from "../../modules/logger";
const logger = new Logger("request-db");

export default class RequestRepository implements IRequestRepository {
    connectionRepository = pg.getRepository<IRequest>(Request);

    async findByRequest(request_from: string, request_to: string): Promise<IRequest> {
        let request: IRequest;
        try {
            request = await this.connectionRepository.findOneBy({ [IRequestEnv.REQUEST_FROM]: request_from, [IRequestEnv.REQUEST_TO]: request_to });
        } catch (err){
            logger.error("Failed exec query findByTarget, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (request === null){
            throw new ApiNotFound(`Not found request with request_from: "${request_from}" request_to: "${request_to}"`);
        }

        return request;
    }
    async create(data: Partial<IRequest>): Promise<void> {
        try {
            await this.connectionRepository.insert(data);
        } catch (err){
            logger.error("Failed exec query create, details:", err);
            throw new ApiErrorGeneric(err);
        }
    }
    async delete(request_from: string, request_to: string): Promise<void> {
        let rs: DeleteResult;
        try {
            rs = await this.connectionRepository.delete({[IRequestEnv.REQUEST_FROM]: request_from, [IRequestEnv.REQUEST_TO]: request_to});
        } catch (err){
            logger.error("Failed exec query delete, details:", err);
            throw new ApiErrorGeneric(err);
        }

        if (rs.affected <= 0){
            throw new ApiNotFound(`Cannot delete request with request_from: "${request_from}" request_to: "${request_to}"`);
        }
    }
    async listPaginated(request_from: string, skip: number = 0, length: number = 10): Promise<Array<IRequest>> {
        let rs: Array<IRequest> = [];
        try {
            rs = await this.connectionRepository.find({where: {[IRequestEnv.REQUEST_FROM]: request_from}, order: {[IRequestEnv.REQUEST_TIME]: "ASC"}, skip, take: length});
        } catch (err){
            logger.error("Failed exec query listPaginated, details:", err);
            throw new ApiErrorGeneric(err);
        }

        return rs;
    }
    async countTotalFromRequestFrom(request_from: string): Promise<number> {
        try {
            return await this.connectionRepository.count({where: {[IRequestEnv.REQUEST_FROM]: request_from}});
        } catch (err){
            logger.error("Failed exec query countTotal, details:", err);
            throw new ApiErrorGeneric(err);
        }
    }
}