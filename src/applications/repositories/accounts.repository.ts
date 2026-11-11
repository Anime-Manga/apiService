import { HttpException, HttpStatus, Injectable } from "@nestjs/common";
import _ from "lodash";
import { db } from "../../prisma/db.js";
import { Account } from "../../persistences/models/accounts.model.js";

@Injectable()
export class AccountsRepository {
    private account = db.orm.public.Account;

    async findByUsername(username: string): Promise<Account> {
        const account = await this.account.first({username});

        if(_.isNil(account)){
            throw new HttpException("Not found account", HttpStatus.NOT_FOUND);
        }

        return account;
    }
}