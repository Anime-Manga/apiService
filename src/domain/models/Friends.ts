import { DateTime } from "luxon";
import { Entity, PrimaryColumn, Column } from "typeorm";

import { IFriend, IFriendEnv } from "../interfaces/models/IFriend";

@Entity("friends")
export class Friend implements IFriend{
    @PrimaryColumn({type: "varchar", length: 250})
    [IFriendEnv.USERNAME]: string;

    @Column({type: "varchar", length: 250})
    [IFriendEnv.FRIEND]: string;

    @Column({type: "timestamptz", default: DateTime.now()})
    [IFriendEnv.BECOME_FRIEND_TIME]: string;
}