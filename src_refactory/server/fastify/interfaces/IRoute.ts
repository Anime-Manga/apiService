import { RouteHandlerMethod, RouteShorthandOptions, HTTPMethods } from "fastify";

export interface IRoute {
    url: string,
    method: HTTPMethods,
    options?: RouteShorthandOptions,
    hander: RouteHandlerMethod
}