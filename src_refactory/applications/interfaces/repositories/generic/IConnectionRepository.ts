import { Repository } from "typeorm";

export default interface IConnectionRepository<T>{
    connectionRepository: Repository<T>;
} // eslint-disable-line @stylistic/semi