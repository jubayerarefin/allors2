// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Commentable } from './Commentable.g';
import { Deletable } from './Deletable.g';
import { RequestItemState } from './RequestItemState.g';
import { RequestItemVersion } from './RequestItemVersion.g';
import { UnitOfMeasure } from './UnitOfMeasure.g';
import { Requirement } from './Requirement.g';
import { Deliverable } from './Deliverable.g';
import { ProductFeature } from './ProductFeature.g';
import { NeededSkill } from './NeededSkill.g';
import { Product } from './Product.g';
import { QuoteItemVersion } from './QuoteItemVersion.g';
import { QuoteItem } from './QuoteItem.g';
import { RequestVersion } from './RequestVersion.g';
import { Request } from './Request.g';

export class RequestItem extends SessionObject implements Commentable, Deletable {
    get CanReadRequestItemState(): boolean {
        return this.canRead('RequestItemState');
    }

    get CanWriteRequestItemState(): boolean {
        return this.canWrite('RequestItemState');
    }

    get RequestItemState(): RequestItemState {
        return this.get('RequestItemState');
    }

    set RequestItemState(value: RequestItemState) {
        this.set('RequestItemState', value);
    }

    get CanReadCurrentVersion(): boolean {
        return this.canRead('CurrentVersion');
    }

    get CanWriteCurrentVersion(): boolean {
        return this.canWrite('CurrentVersion');
    }

    get CurrentVersion(): RequestItemVersion {
        return this.get('CurrentVersion');
    }

    set CurrentVersion(value: RequestItemVersion) {
        this.set('CurrentVersion', value);
    }

    get CanReadAllVersions(): boolean {
        return this.canRead('AllVersions');
    }

    get CanWriteAllVersions(): boolean {
        return this.canWrite('AllVersions');
    }

    get AllVersions(): RequestItemVersion[] {
        return this.get('AllVersions');
    }

    AddAllVersion(value: RequestItemVersion) {
        return this.add('AllVersions', value);
    }

    RemoveAllVersion(value: RequestItemVersion) {
        return this.remove('AllVersions', value);
    }

    set AllVersions(value: RequestItemVersion[]) {
        this.set('AllVersions', value);
    }

    get CanReadInternalComment(): boolean {
        return this.canRead('InternalComment');
    }

    get CanWriteInternalComment(): boolean {
        return this.canWrite('InternalComment');
    }

    get InternalComment(): string {
        return this.get('InternalComment');
    }

    set InternalComment(value: string) {
        this.set('InternalComment', value);
    }

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

    get CanReadQuantity(): boolean {
        return this.canRead('Quantity');
    }

    get CanWriteQuantity(): boolean {
        return this.canWrite('Quantity');
    }

    get Quantity(): number {
        return this.get('Quantity');
    }

    set Quantity(value: number) {
        this.set('Quantity', value);
    }

    get CanReadUnitOfMeasure(): boolean {
        return this.canRead('UnitOfMeasure');
    }

    get CanWriteUnitOfMeasure(): boolean {
        return this.canWrite('UnitOfMeasure');
    }

    get UnitOfMeasure(): UnitOfMeasure {
        return this.get('UnitOfMeasure');
    }

    set UnitOfMeasure(value: UnitOfMeasure) {
        this.set('UnitOfMeasure', value);
    }

    get CanReadRequirements(): boolean {
        return this.canRead('Requirements');
    }

    get CanWriteRequirements(): boolean {
        return this.canWrite('Requirements');
    }

    get Requirements(): Requirement[] {
        return this.get('Requirements');
    }

    AddRequirement(value: Requirement) {
        return this.add('Requirements', value);
    }

    RemoveRequirement(value: Requirement) {
        return this.remove('Requirements', value);
    }

    set Requirements(value: Requirement[]) {
        this.set('Requirements', value);
    }

    get CanReadDeliverable(): boolean {
        return this.canRead('Deliverable');
    }

    get CanWriteDeliverable(): boolean {
        return this.canWrite('Deliverable');
    }

    get Deliverable(): Deliverable {
        return this.get('Deliverable');
    }

    set Deliverable(value: Deliverable) {
        this.set('Deliverable', value);
    }

    get CanReadProductFeature(): boolean {
        return this.canRead('ProductFeature');
    }

    get CanWriteProductFeature(): boolean {
        return this.canWrite('ProductFeature');
    }

    get ProductFeature(): ProductFeature {
        return this.get('ProductFeature');
    }

    set ProductFeature(value: ProductFeature) {
        this.set('ProductFeature', value);
    }

    get CanReadNeededSkill(): boolean {
        return this.canRead('NeededSkill');
    }

    get CanWriteNeededSkill(): boolean {
        return this.canWrite('NeededSkill');
    }

    get NeededSkill(): NeededSkill {
        return this.get('NeededSkill');
    }

    set NeededSkill(value: NeededSkill) {
        this.set('NeededSkill', value);
    }

    get CanReadProduct(): boolean {
        return this.canRead('Product');
    }

    get CanWriteProduct(): boolean {
        return this.canWrite('Product');
    }

    get Product(): Product {
        return this.get('Product');
    }

    set Product(value: Product) {
        this.set('Product', value);
    }

    get CanReadMaximumAllowedPrice(): boolean {
        return this.canRead('MaximumAllowedPrice');
    }

    get CanWriteMaximumAllowedPrice(): boolean {
        return this.canWrite('MaximumAllowedPrice');
    }

    get MaximumAllowedPrice(): number {
        return this.get('MaximumAllowedPrice');
    }

    set MaximumAllowedPrice(value: number) {
        this.set('MaximumAllowedPrice', value);
    }

    get CanReadRequiredByDate(): boolean {
        return this.canRead('RequiredByDate');
    }

    get CanWriteRequiredByDate(): boolean {
        return this.canWrite('RequiredByDate');
    }

    get RequiredByDate(): Date {
        return this.get('RequiredByDate');
    }

    set RequiredByDate(value: Date) {
        this.set('RequiredByDate', value);
    }

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


    get CanExecuteCancel(): boolean {
        return this.canExecute('Cancel');
    }

    get Cancel(): Method {
        return new Method(this, 'Cancel');
    }
    get CanExecuteSubmit(): boolean {
        return this.canExecute('Submit');
    }

    get Submit(): Method {
        return new Method(this, 'Submit');
    }
    get CanExecuteHold(): boolean {
        return this.canExecute('Hold');
    }

    get Hold(): Method {
        return new Method(this, 'Hold');
    }
    get CanExecuteDelete(): boolean {
        return this.canExecute('Delete');
    }

    get Delete(): Method {
        return new Method(this, 'Delete');
    }
}
