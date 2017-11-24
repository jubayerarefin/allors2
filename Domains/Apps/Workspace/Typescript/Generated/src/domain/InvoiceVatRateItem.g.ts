// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { VatRate } from './VatRate.g';
import { InvoiceItemVersion } from './InvoiceItemVersion.g';
import { InvoiceItem } from './InvoiceItem.g';

export class InvoiceVatRateItem extends SessionObject  {
    get CanReadBaseAmount(): boolean {
        return this.canRead('BaseAmount');
    }

    get CanWriteBaseAmount(): boolean {
        return this.canWrite('BaseAmount');
    }

    get BaseAmount(): number {
        return this.get('BaseAmount');
    }

    set BaseAmount(value: number) {
        this.set('BaseAmount', value);
    }

    get CanReadVatRates(): boolean {
        return this.canRead('VatRates');
    }

    get CanWriteVatRates(): boolean {
        return this.canWrite('VatRates');
    }

    get VatRates(): VatRate[] {
        return this.get('VatRates');
    }

    AddVatRate(value: VatRate) {
        return this.add('VatRates', value);
    }

    RemoveVatRate(value: VatRate) {
        return this.remove('VatRates', value);
    }

    set VatRates(value: VatRate[]) {
        this.set('VatRates', value);
    }

    get CanReadVatAmount(): boolean {
        return this.canRead('VatAmount');
    }

    get CanWriteVatAmount(): boolean {
        return this.canWrite('VatAmount');
    }

    get VatAmount(): number {
        return this.get('VatAmount');
    }

    set VatAmount(value: number) {
        this.set('VatAmount', value);
    }


}
