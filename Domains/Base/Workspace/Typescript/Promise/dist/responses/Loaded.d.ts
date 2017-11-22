import { ISession, ISessionObject, PullResponse } from "@allors/base-domain";
export declare class Loaded {
    session: ISession;
    response: PullResponse;
    objects: {
        [name: string]: ISessionObject;
    };
    collections: {
        [name: string]: ISessionObject[];
    };
    values: {
        [name: string]: any;
    };
    constructor(session: ISession, response: PullResponse);
}
