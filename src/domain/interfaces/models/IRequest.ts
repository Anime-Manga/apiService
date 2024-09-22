import { Type } from "@sinclair/typebox";

export enum IRequestEnv {
    REQUEST_TO = "request_to",
    REQUEST_FROM = "request_from",
    REQUEST_TIME = "request_time"
}

export const requestSchema = Type.Object({
    [IRequestEnv.REQUEST_FROM]: Type.String({maxLength: 250}),
    [IRequestEnv.REQUEST_TO]: Type.String({maxLength: 250}),
    [IRequestEnv.REQUEST_TIME]: Type.String({format: "date-time"})
});

export type IRequest = typeof requestSchema.static