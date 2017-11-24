// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Deletable } from './Deletable.g';
import { Localised } from './Localised.g';
import { ProductCharacteristic } from './ProductCharacteristic.g';
import { Locale } from './Locale.g';
import { InventoryItemVersion } from './InventoryItemVersion.g';
import { InventoryItem } from './InventoryItem.g';

export class ProductCharacteristicValue extends SessionObject implements Deletable, Localised {
    get CanReadProductCharacteristic(): boolean {
        return this.canRead('ProductCharacteristic');
    }

    get CanWriteProductCharacteristic(): boolean {
        return this.canWrite('ProductCharacteristic');
    }

    get ProductCharacteristic(): ProductCharacteristic {
        return this.get('ProductCharacteristic');
    }

    set ProductCharacteristic(value: ProductCharacteristic) {
        this.set('ProductCharacteristic', value);
    }

    get CanReadValue(): boolean {
        return this.canRead('Value');
    }

    get CanWriteValue(): boolean {
        return this.canWrite('Value');
    }

    get Value(): string {
        return this.get('Value');
    }

    set Value(value: string) {
        this.set('Value', value);
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


    get CanExecuteDelete(): boolean {
        return this.canExecute('Delete');
    }

    get Delete(): Method {
        return new Method(this, 'Delete');
    }
}
