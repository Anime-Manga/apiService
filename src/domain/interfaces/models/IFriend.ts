import { Type } from "@sinclair/typebox";

export enum IFriendEnv {
    USERNAME = "username",
    FRIEND = "friend",
    BECOME_FRIEND_TIME = "become_friend_time"
}

export const friendSchema = Type.Object({
    [IFriendEnv.USERNAME]: Type.String({ maxLength: 250 }),
    [IFriendEnv.FRIEND]: Type.String({ maxLength: 250 }),
    [IFriendEnv.BECOME_FRIEND_TIME]: Type.String({format: "date-time"})
});

export type IFriend = typeof friendSchema.static