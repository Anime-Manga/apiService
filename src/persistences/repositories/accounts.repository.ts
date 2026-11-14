import _ from "lodash";
import { HttpException, HttpStatus, Injectable } from "@nestjs/common";

import { Account } from "../../domain/models/index.js";
import postgresqlUtils from "../../utils/postgresql.utils.js";

@Injectable()
export class AccountsRepository {
    private account = postgresqlUtils.getClient().account;

    async findByUsername(username: string): Promise<Account> {
        const account = await this.account.findFirst({
            where: {
                username
            }
        });

        if(_.isNil(account)){
            throw new HttpException("Not found account", HttpStatus.NOT_FOUND);
        }

        return account;
    }

    async create(account: Pick<Account, "username" | "password"> & Partial<Pick<Account, "role" | "enabled">>){
        await this.account.create({
            data: account
        });
    }
}