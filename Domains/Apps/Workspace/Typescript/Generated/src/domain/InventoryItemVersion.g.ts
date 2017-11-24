// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Version } from './Version.g';
import { ProductCharacteristicValue } from './ProductCharacteristicValue.g';
import { InventoryItemVariance } from './InventoryItemVariance.g';
import { Part } from './Part.g';
import { Lot } from './Lot.g';
import { UnitOfMeasure } from './UnitOfMeasure.g';
import { Good } from './Good.g';
import { ProductType } from './ProductType.g';
import { Facility } from './Facility.g';

export interface InventoryItemVersion extends SessionObject , Version {
        ProductCharacteristicValues : ProductCharacteristicValue[];
        AddProductCharacteristicValue(value: ProductCharacteristicValue);
        RemoveProductCharacteristicValue(value: ProductCharacteristicValue);


        InventoryItemVariances : InventoryItemVariance[];
        AddInventoryItemVariance(value: InventoryItemVariance);
        RemoveInventoryItemVariance(value: InventoryItemVariance);


        Part : Part;


        Name : string;


        Lot : Lot;


        Sku : string;


        UnitOfMeasure : UnitOfMeasure;


        Good : Good;


        ProductType : ProductType;


        Facility : Facility;


}