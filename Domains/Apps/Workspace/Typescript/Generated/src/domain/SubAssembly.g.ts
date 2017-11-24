// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Part } from './Part.g';
import { UniquelyIdentifiable } from './UniquelyIdentifiable.g';
import { InventoryItemVersion } from './InventoryItemVersion.g';
import { InventoryItem } from './InventoryItem.g';

export class SubAssembly extends SessionObject implements Part {
    get CanReadUniqueId(): boolean {
        return this.canRead('UniqueId');
    }

    get CanWriteUniqueId(): boolean {
        return this.canWrite('UniqueId');
    }

    get UniqueId(): string {
        return this.get('UniqueId');
    }

    set UniqueId(value: string) {
        this.set('UniqueId', value);
    }


}
