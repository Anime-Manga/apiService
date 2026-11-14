import { HttpException, HttpStatus, Injectable } from "@nestjs/common";
import { AccountsRepository } from "../../persistences/repositories/accounts.repository.js";
import accountsUtils from "../../utils/accounts.utils.js";
import { AccountDTO, AccountToAccountDTO } from "../../domain/dto/account.dto.js";
import { Account } from "../../domain/models/index.js";

@Injectable()
export class AccountsService {
    constructor(
        private readonly accountsRepository: AccountsRepository
    ){}

    async login(username: string, password: string): Promise<AccountDTO>{
        
        let account;
        try{
            account = await this.accountsRepository.findByUsername(username)
        }catch(err){
            if(err instanceof HttpException && err.getStatus() === HttpStatus.NOT_FOUND){
                throw new HttpException("Username and/or Password incorrect", HttpStatus.UNAUTHORIZED);
            }
            
            throw err;
        }

        if(accountsUtils.encryptPassword(password) !== account.password){
            throw new HttpException("Username and/or Password incorrect", HttpStatus.UNAUTHORIZED);
        }

        return AccountToAccountDTO(account);
    }

    async create(account: Pick<Account, "username" | "password">): Promise<void>{
        try{
            await this.accountsRepository.findByUsername(account.username);
        }catch(err){
            if((err instanceof HttpException && err.getStatus() !== HttpStatus.NOT_FOUND) || err){
                throw err;
            }
        }

        await this.accountsRepository.create({username: account.username, password: accountsUtils.encryptPassword(account.password)});
    }
}