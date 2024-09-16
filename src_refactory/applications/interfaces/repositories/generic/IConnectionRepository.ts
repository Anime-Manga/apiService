import { Repository } from "typeorm";

export default interface IConnectionRepository<T>{
    connectionRepository: Repository<T>;
}