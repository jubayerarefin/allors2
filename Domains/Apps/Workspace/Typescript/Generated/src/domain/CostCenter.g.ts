// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { UniquelyIdentifiable } from './UniquelyIdentifiable.g';
import { OrganisationGlAccount } from './OrganisationGlAccount.g';
import { CostCenterCategory } from './CostCenterCategory.g';
import { GeneralLedgerAccount } from './GeneralLedgerAccount.g';

export class CostCenter extends SessionObject implements UniquelyIdentifiable {
    get CanReadDescription(): boolean {
        return this.canRead('Description');
    }

    get CanWriteDescription(): boolean {
        return this.canWrite('Description');
    }

    get Description(): string {
        return this.get('Description');
    }

    set Description(value: string) {
        this.set('Description', value);
    }

    get CanReadInternalTransferGlAccount(): boolean {
        return this.canRead('InternalTransferGlAccount');
    }

    get CanWriteInternalTransferGlAccount(): boolean {
        return this.canWrite('InternalTransferGlAccount');
    }

    get InternalTransferGlAccount(): OrganisationGlAccount {
        return this.get('InternalTransferGlAccount');
    }

    set InternalTransferGlAccount(value: OrganisationGlAccount) {
        this.set('InternalTransferGlAccount', value);
    }

    get CanReadCostCenterCategories(): boolean {
        return this.canRead('CostCenterCategories');
    }

    get CanWriteCostCenterCategories(): boolean {
        return this.canWrite('CostCenterCategories');
    }

    get CostCenterCategories(): CostCenterCategory[] {
        return this.get('CostCenterCategories');
    }

    AddCostCenterCategory(value: CostCenterCategory) {
        return this.add('CostCenterCategories', value);
    }

    RemoveCostCenterCategory(value: CostCenterCategory) {
        return this.remove('CostCenterCategories', value);
    }

    set CostCenterCategories(value: CostCenterCategory[]) {
        this.set('CostCenterCategories', value);
    }

    get CanReadRedistributedCostsGlAccount(): boolean {
        return this.canRead('RedistributedCostsGlAccount');
    }

    get CanWriteRedistributedCostsGlAccount(): boolean {
        return this.canWrite('RedistributedCostsGlAccount');
    }

    get RedistributedCostsGlAccount(): OrganisationGlAccount {
        return this.get('RedistributedCostsGlAccount');
    }

    set RedistributedCostsGlAccount(value: OrganisationGlAccount) {
        this.set('RedistributedCostsGlAccount', value);
    }

    get CanReadName(): boolean {
        return this.canRead('Name');
    }

    get CanWriteName(): boolean {
        return this.canWrite('Name');
    }

    get Name(): string {
        return this.get('Name');
    }

    set Name(value: string) {
        this.set('Name', value);
    }

    get CanReadActive(): boolean {
        return this.canRead('Active');
    }

    get CanWriteActive(): boolean {
        return this.canWrite('Active');
    }

    get Active(): boolean {
        return this.get('Active');
    }

    set Active(value: boolean) {
        this.set('Active', value);
    }

    get CanReadUseGlAccountOfBooking(): boolean {
        return this.canRead('UseGlAccountOfBooking');
    }

    get CanWriteUseGlAccountOfBooking(): boolean {
        return this.canWrite('UseGlAccountOfBooking');
    }

    get UseGlAccountOfBooking(): boolean {
        return this.get('UseGlAccountOfBooking');
    }

    set UseGlAccountOfBooking(value: boolean) {
        this.set('UseGlAccountOfBooking', value);
    }

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
