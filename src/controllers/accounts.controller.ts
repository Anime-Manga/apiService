import z from "zod";
import { Controller, Delete, Post, Query } from "@nestjs/common";

import { AccountsService } from "../applications/services/accounts.service.js";
import { AccountDTO } from "../persistences/dto/account.dto.js";

@Controller("accounts")
export class AccountsController {
    constructor(private readonly accountsService: AccountsService) {}

    @Post("login")
    async login(
        @Query({
            schema: z.object({
                username: z.string().nonempty(),
                password: z.string().nonempty()
            })
        }) query: {username: string, password: string},
    ): Promise<AccountDTO>{
        const {password, username} = query;
        return await this.accountsService.login(username, password);
    }

    @Post("create")
    create(){
        
    }

    @Post("watchlist")
    addWatchlist(){

    }

    @Delete("watchlist")
    deleteWatchlist(){

    }
}