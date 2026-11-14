import z from "zod";
import { Body, Controller, Delete, Post, Query } from "@nestjs/common";

import { ISchemaVoid } from "../schemas/generic.js";
import { AccountDTO } from "../domain/dto/account.dto.js";
import { AccountsService } from "../applications/services/accounts.service.js";
import { AccountSchema } from "../domain/models/index.js";

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
    async create(
        @Body(AccountSchema.pick({username: true, password: true})) body: {username: string, password: string},
    ): Promise<ISchemaVoid>{
        await this.accountsService.create(body);
        return { status: "ok" }
    }
}