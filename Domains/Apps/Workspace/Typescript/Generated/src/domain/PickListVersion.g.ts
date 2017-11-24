// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Version } from './Version.g';
import { PickListState } from './PickListState.g';
import { CustomerShipment } from './CustomerShipment.g';
import { PickListItem } from './PickListItem.g';
import { Person } from './Person.g';
import { Party } from './Party.g';
import { Store } from './Store.g';
import { PickList } from './PickList.g';

export class PickListVersion extends SessionObject implements Version {
    get CanReadPickListState(): boolean {
        return this.canRead('PickListState');
    }

    get PickListState(): PickListState {
        return this.get('PickListState');
    }


    get CanReadCustomerShipmentCorrection(): boolean {
        return this.canRead('CustomerShipmentCorrection');
    }

    get CanWriteCustomerShipmentCorrection(): boolean {
        return this.canWrite('CustomerShipmentCorrection');
    }

    get CustomerShipmentCorrection(): CustomerShipment {
        return this.get('CustomerShipmentCorrection');
    }

    set CustomerShipmentCorrection(value: CustomerShipment) {
        this.set('CustomerShipmentCorrection', value);
    }

    get CanReadCreationDate(): boolean {
        return this.canRead('CreationDate');
    }

    get CanWriteCreationDate(): boolean {
        return this.canWrite('CreationDate');
    }

    get CreationDate(): Date {
        return this.get('CreationDate');
    }

    set CreationDate(value: Date) {
        this.set('CreationDate', value);
    }

    get CanReadPickListItems(): boolean {
        return this.canRead('PickListItems');
    }

    get CanWritePickListItems(): boolean {
        return this.canWrite('PickListItems');
    }

    get PickListItems(): PickListItem[] {
        return this.get('PickListItems');
    }

    AddPickListItem(value: PickListItem) {
        return this.add('PickListItems', value);
    }

    RemovePickListItem(value: PickListItem) {
        return this.remove('PickListItems', value);
    }

    set PickListItems(value: PickListItem[]) {
        this.set('PickListItems', value);
    }

    get CanReadPicker(): boolean {
        return this.canRead('Picker');
    }

    get CanWritePicker(): boolean {
        return this.canWrite('Picker');
    }

    get Picker(): Person {
        return this.get('Picker');
    }

    set Picker(value: Person) {
        this.set('Picker', value);
    }

    get CanReadShipToParty(): boolean {
        return this.canRead('ShipToParty');
    }

    get CanWriteShipToParty(): boolean {
        return this.canWrite('ShipToParty');
    }

    get ShipToParty(): Party {
        return this.get('ShipToParty');
    }

    set ShipToParty(value: Party) {
        this.set('ShipToParty', value);
    }

    get CanReadStore(): boolean {
        return this.canRead('Store');
    }

    get CanWriteStore(): boolean {
        return this.canWrite('Store');
    }

    get Store(): Store {
        return this.get('Store');
    }

    set Store(value: Store) {
        this.set('Store', value);
    }

    get CanReadDerivationTimeStamp(): boolean {
        return this.canRead('DerivationTimeStamp');
    }

    get CanWriteDerivationTimeStamp(): boolean {
        return this.canWrite('DerivationTimeStamp');
    }

    get DerivationTimeStamp(): Date {
        return this.get('DerivationTimeStamp');
    }

    set DerivationTimeStamp(value: Date) {
        this.set('DerivationTimeStamp', value);
    }


}
