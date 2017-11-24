// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { UniquelyIdentifiable } from './UniquelyIdentifiable.g';
import { Package } from './Package.g';
import { LocalisedText } from './LocalisedText.g';
import { Media } from './Media.g';
import { CatScope } from './CatScope.g';
import { Product } from './Product.g';
import { Brand } from './Brand.g';
import { Catalogue } from './Catalogue.g';
import { OrganisationGlAccount } from './OrganisationGlAccount.g';
import { SalesRepRelationship } from './SalesRepRelationship.g';

export class ProductCategory extends SessionObject implements UniquelyIdentifiable {
    get CanReadPackage(): boolean {
        return this.canRead('Package');
    }

    get CanWritePackage(): boolean {
        return this.canWrite('Package');
    }

    get Package(): Package {
        return this.get('Package');
    }

    set Package(value: Package) {
        this.set('Package', value);
    }

    get CanReadCode(): boolean {
        return this.canRead('Code');
    }

    get CanWriteCode(): boolean {
        return this.canWrite('Code');
    }

    get Code(): string {
        return this.get('Code');
    }

    set Code(value: string) {
        this.set('Code', value);
    }

    get CanReadParents(): boolean {
        return this.canRead('Parents');
    }

    get CanWriteParents(): boolean {
        return this.canWrite('Parents');
    }

    get Parents(): ProductCategory[] {
        return this.get('Parents');
    }

    AddParent(value: ProductCategory) {
        return this.add('Parents', value);
    }

    RemoveParent(value: ProductCategory) {
        return this.remove('Parents', value);
    }

    set Parents(value: ProductCategory[]) {
        this.set('Parents', value);
    }

    get CanReadChildren(): boolean {
        return this.canRead('Children');
    }

    get Children(): ProductCategory[] {
        return this.get('Children');
    }


    get CanReadDescription(): boolean {
        return this.canRead('Description');
    }

    get Description(): string {
        return this.get('Description');
    }


    get CanReadName(): boolean {
        return this.canRead('Name');
    }

    get Name(): string {
        return this.get('Name');
    }


    get CanReadLocalisedNames(): boolean {
        return this.canRead('LocalisedNames');
    }

    get CanWriteLocalisedNames(): boolean {
        return this.canWrite('LocalisedNames');
    }

    get LocalisedNames(): LocalisedText[] {
        return this.get('LocalisedNames');
    }

    AddLocalisedName(value: LocalisedText) {
        return this.add('LocalisedNames', value);
    }

    RemoveLocalisedName(value: LocalisedText) {
        return this.remove('LocalisedNames', value);
    }

    set LocalisedNames(value: LocalisedText[]) {
        this.set('LocalisedNames', value);
    }

    get CanReadLocalisedDescriptions(): boolean {
        return this.canRead('LocalisedDescriptions');
    }

    get CanWriteLocalisedDescriptions(): boolean {
        return this.canWrite('LocalisedDescriptions');
    }

    get LocalisedDescriptions(): LocalisedText[] {
        return this.get('LocalisedDescriptions');
    }

    AddLocalisedDescription(value: LocalisedText) {
        return this.add('LocalisedDescriptions', value);
    }

    RemoveLocalisedDescription(value: LocalisedText) {
        return this.remove('LocalisedDescriptions', value);
    }

    set LocalisedDescriptions(value: LocalisedText[]) {
        this.set('LocalisedDescriptions', value);
    }

    get CanReadCategoryImage(): boolean {
        return this.canRead('CategoryImage');
    }

    get CanWriteCategoryImage(): boolean {
        return this.canWrite('CategoryImage');
    }

    get CategoryImage(): Media {
        return this.get('CategoryImage');
    }

    set CategoryImage(value: Media) {
        this.set('CategoryImage', value);
    }

    get CanReadSuperJacent(): boolean {
        return this.canRead('SuperJacent');
    }

    get SuperJacent(): ProductCategory[] {
        return this.get('SuperJacent');
    }


    get CanReadCatScope(): boolean {
        return this.canRead('CatScope');
    }

    get CanWriteCatScope(): boolean {
        return this.canWrite('CatScope');
    }

    get CatScope(): CatScope {
        return this.get('CatScope');
    }

    set CatScope(value: CatScope) {
        this.set('CatScope', value);
    }

    get CanReadAllProducts(): boolean {
        return this.canRead('AllProducts');
    }

    get AllProducts(): Product[] {
        return this.get('AllProducts');
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
