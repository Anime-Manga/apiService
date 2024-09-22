import { ISchemaReturnPaginatedRequest } from "../../../schemas/requestSchema";

export default interface IRequestService {
    //get
    listRequestWaitPaginated(request_from: string, skip: number, length: number): Promise<ISchemaReturnPaginatedRequest>
    listRequestForActionPaginated(request_to: string, skip: number, length: number): Promise<ISchemaReturnPaginatedRequest>
    
    //post
    create(auth_username: string, request_to: string): Promise<void>
    action(request_from: string, auth_username: string, accept: boolean): Promise<void>

    //delete
    delete(auth_username: string, request_to: string): Promise<void>

} // eslint-disable-line @stylistic/semi