import { IRequest } from "../../../domain/interfaces/models/IRequest"
import { staticReturnPaginatedRequest } from "../../../schemas/requestSchema"

export default interface IRequestService {
    //get
    listPaginated(request_from: string, skip: number, length: number): Promise<staticReturnPaginatedRequest>
    
    //put
    create(data: Partial<IRequest>): Promise<void>

    //post
    action(request_from: string, request_to: string, accept: boolean): Promise<void>
}