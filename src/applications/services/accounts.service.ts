import { HttpException, HttpStatus, Injectable } from "@nestjs/common";
import { AccountsRepository } from "../repositories/accounts.repository.js";
import accountsUtils from "../../utils/accounts.utils.js";
import { AccountDTO, AccountToAccountDTO } from "../../persistences/dto/account.dto.js";

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
}