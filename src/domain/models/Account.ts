import { DateTime } from "luxon";
import { Entity, PrimaryColumn, Column } from "typeorm";

import { IAccount, IAccountEnv } from "../interfaces/models/IAccount";

@Entity("accounts")
export class Account implements IAccount{
    @PrimaryColumn({type: "varchar", length: 250})
    [IAccountEnv.USERNAME]: string;

    @Column({type: "varchar", length: 500})
    [IAccountEnv.PASSWORD]: string;

    @Column({type: "timestamptz", nullable: true, default: null})
    [IAccountEnv.LAST_ACCESS]: string | null;
    
    @Column({type: "boolean", default: false})
    [IAccountEnv.CHANGE_PASSWORD]: boolean;
    
    @Column({type: "timestamptz", default: DateTime.now().plus({month: 1})})
    [IAccountEnv.EXPIRE_PASSWORD]: string;
    
    @Column({type: "text", default: null, nullable: true})
    [IAccountEnv.PROFILE]: string;
}