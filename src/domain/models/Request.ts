import { DateTime } from "luxon";
import { Entity, PrimaryColumn, Column } from "typeorm";

import { IRequest, IRequestEnv } from "../interfaces/models/IRequest";

@Entity("requests")
export class Request implements IRequest{
    @PrimaryColumn({type: "varchar", length: 250})
    [IRequestEnv.REQUEST_FROM]: string;

    @PrimaryColumn({type: "varchar", length: 250})
    [IRequestEnv.REQUEST_TO]: string;

    @Column({type: "timestamptz", default: DateTime.now()})
    [IRequestEnv.REQUEST_TIME]: string;
}