import { IRequest } from "../../../domain/interfaces/models/IRequest"
import { Request } from "../../../domain/models/Request";
import IConnectionRepository from "./generic/IConnectionRepository";

export default interface IRequestRepository extends IConnectionRepository<Request> {
    //get
    findByRequest(request_from: string, request_to: string): Promise<IRequest>
    listPaginated(request_from: string, skip: number, length: number): Promise<Array<IRequest>>
    countTotalFromRequestFrom(request_from: string): Promise<number>

    //put
    create(data: IRequest): Promise<void>
    
    //delete
    delete(request_from: string, request_to: string): Promise<void>
}