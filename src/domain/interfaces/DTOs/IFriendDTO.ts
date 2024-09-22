import { Type } from "@sinclair/typebox";

export enum IFriendDTOEnv {
    USERNAME = "username",
    FRIEND = "friend",
    BECOME_FRIEND_TIME = "become_friend_time"
}

export const friendDTOSchema = Type.Object({
    [IFriendDTOEnv.USERNAME]: Type.String({ maxLength: 250 }),
    [IFriendDTOEnv.FRIEND]: Type.String({ maxLength: 250 }),
    [IFriendDTOEnv.BECOME_FRIEND_TIME]: Type.String({format: "date-time"})
});

export type IFriendDTO = typeof friendDTOSchema.static