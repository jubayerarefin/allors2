// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { InventoryItemConfiguration } from './InventoryItemConfiguration.g';
import { Commentable } from './Commentable.g';

export class ServiceConfiguration extends SessionObject implements InventoryItemConfiguration {
    get CanReadComment(): boolean {
        return this.canRead('Comment');
    }

    get CanWriteComment(): boolean {
        return this.canWrite('Comment');
    }

    get Comment(): string {
        return this.get('Comment');
    }

    set Comment(value: string) {
        this.set('Comment', value);
    }


}
