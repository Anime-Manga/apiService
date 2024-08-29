import { IRoute } from "./IRoute";

export interface IController {
    routes: {[key: string]: IRoute},
    pathBaseController: string
}