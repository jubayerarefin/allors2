// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { UniquelyIdentifiable } from './UniquelyIdentifiable.g';
import { Localised } from './Localised.g';
import { Locale } from './Locale.g';

export class StringTemplate extends SessionObject implements UniquelyIdentifiable, Localised {
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

    get CanReadLocale(): boolean {
        return this.canRead('Locale');
    }

    get CanWriteLocale(): boolean {
        return this.canWrite('Locale');
    }

    get Locale(): Locale {
        return this.get('Locale');
    }

    set Locale(value: Locale) {
        this.set('Locale', value);
    }


}
