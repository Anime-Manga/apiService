import { Type } from "@sinclair/typebox";

export enum IRequestDTOEnv {
    REQUEST_TO = "request_to",
    REQUEST_FROM = "request_from",
    REQUEST_TIME = "request_time"
}

export const requestDTOSchema = Type.Object({
    [IRequestDTOEnv.REQUEST_FROM]: Type.String({maxLength: 250}),
    [IRequestDTOEnv.REQUEST_TO]: Type.String({maxLength: 250}),
    [IRequestDTOEnv.REQUEST_TIME]: Type.String({format: 'date-time'}),
})

export type IRequestDTO = typeof requestDTOSchema.static