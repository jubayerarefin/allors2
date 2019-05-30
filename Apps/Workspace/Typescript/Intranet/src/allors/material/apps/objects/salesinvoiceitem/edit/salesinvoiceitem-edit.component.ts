import * as moment from 'moment';

import { Component, OnDestroy, OnInit, Self, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { Subscription, combineLatest } from 'rxjs';

import { SearchFactory, ContextService, MetaService, RefreshService, TestScope } from '../../../../../angular';
import { Facility, NonUnifiedGood, InventoryItem, InvoiceItemType, NonSerialisedInventoryItem, Product, SalesInvoice, SalesInvoiceItem, SalesOrderItem, SerialisedInventoryItem, VatRate, VatRegime, SerialisedItem, Part, NonUnifiedPart, SupplierOffering } from '../../../../../domain';
import { And, Equals, PullRequest, Sort, Filter, IObject } from '../../../../../framework';
import { ObjectData } from '../../../../../material/base/services/object';
import { Meta } from '../../../../../meta';
import { switchMap, map } from 'rxjs/operators';
import { SaveService, FiltersService } from '../../../../../material';

@Component({
  templateUrl: './salesinvoiceitem-edit.component.html',
  providers: [ContextService]

})
export class SalesInvoiceItemEditComponent extends TestScope implements OnInit, OnDestroy {

  readonly m: Meta;

  title: string;
  invoice: SalesInvoice;
  invoiceItem: SalesInvoiceItem;
  orderItem: SalesOrderItem;
  inventoryItems: InventoryItem[];
  vatRates: VatRate[];
  vatRegimes: VatRegime[];
  serialisedInventoryItem: SerialisedInventoryItem;
  nonSerialisedInventoryItem: NonSerialisedInventoryItem;
  goods: NonUnifiedGood[];
  invoiceItemTypes: InvoiceItemType[];
  productItemType: InvoiceItemType;
  facilities: Facility[];
  goodsFacilityFilter: SearchFactory;
  part: Part;
  serialisedItem: SerialisedItem;
  serialisedItems: SerialisedItem[] = [];

  private previousProduct;
  private subscription: Subscription;
  parts: NonUnifiedPart[];
  partItemType: InvoiceItemType;
  supplierOffering: SupplierOffering;

  constructor(
    @Self() public allors: ContextService,
    @Inject(MAT_DIALOG_DATA) public data: ObjectData,
    public filtersService: FiltersService,
    public dialogRef: MatDialogRef<SalesInvoiceItemEditComponent>,
    public refreshService: RefreshService,
    public metaService: MetaService,
    private saveService: SaveService,
  ) {
    super();

    this.m = this.metaService.m;

    this.goodsFacilityFilter = new SearchFactory({
      objectType: this.m.Good,
      roleTypes: [this.m.Good.Name],
    });
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = combineLatest(this.refreshService.refresh$)
      .pipe(
        switchMap(([]) => {

          const isCreate = this.data.id === undefined;
          const { id } = this.data;

          const pulls = [
            pull.SalesInvoiceItem({
              object: id,
              include:
              {
                SalesInvoiceItemState: x,
                SerialisedItem: x,
                Facility: {
                  Owner: x,
                },
                VatRegime: {
                  VatRate: x,
                }
              }
            }),
            pull.NonUnifiedGood({
              sort: [
                new Sort(m.NonUnifiedGood.Name),
              ],
            }),
            pull.SalesInvoiceItem({
              object: id,
              fetch: {
                SalesInvoiceWhereSalesInvoiceItem: {
                  include: {
                    VatRegime: x
                  }
                }
              }
            }),
            pull.InvoiceItemType({
              predicate: new Equals({ propertyType: m.InvoiceItemType.IsActive, value: true }),
            }),
            pull.VatRate(),
            pull.VatRegime(),
            pull.Facility({
              include: {
                Owner: x
              },
              sort: [
                new Sort(m.Facility.Name),
              ],
            })
          ];

          if (this.data.associationId) {
            pulls.push(
              pull.SalesInvoice({
                object: this.data.associationId,
                include: {
                  VatRegime: x
                }
              })
            );
          }

          return this.allors.context.load('Pull', new PullRequest({ pulls }))
            .pipe(
              map((loaded) => ({ loaded, isCreate }))
            );
        })
      )
      .subscribe(({ loaded, isCreate }) => {
        this.allors.context.reset();

        this.invoice = loaded.objects.SalesInvoice as SalesInvoice;
        this.invoiceItem = loaded.objects.SalesInvoiceItem as SalesInvoiceItem;
        this.orderItem = loaded.objects.SalesOrderItem as SalesOrderItem;
        this.goods = loaded.collections.NonUnifiedGoods as NonUnifiedGood[];
        this.parts = loaded.collections.NonUnifiedParts as NonUnifiedPart[];
        this.vatRates = loaded.collections.VatRates as VatRate[];
        this.vatRegimes = loaded.collections.VatRegimes as VatRegime[];
        this.facilities = loaded.collections.Facilities as Facility[];
        this.invoiceItemTypes = loaded.collections.InvoiceItemTypes as InvoiceItemType[];
        this.productItemType = this.invoiceItemTypes.find((v: InvoiceItemType) => v.UniqueId === '0d07f778273544cb8354fb887ada42ad');
        this.partItemType = this.invoiceItemTypes.find((v: InvoiceItemType) => v.UniqueId === 'ff2b943d57c943119c569ff37959653b');

        if (isCreate) {
          this.title = 'Add sales invoice Item';
          this.invoiceItem = this.allors.context.create('SalesInvoiceItem') as SalesInvoiceItem;
          this.invoice.AddSalesInvoiceItem(this.invoiceItem);
        } else {
          this.title = 'Edit invoice Item';

          this.previousProduct = this.invoiceItem.Product;
          this.serialisedItem = this.invoiceItem.SerialisedItem;

          if (this.invoiceItem.InvoiceItemType === this.productItemType) {
            this.goodSelected(this.invoiceItem.Product);
          }

          if (this.invoiceItem.CanWriteQuantity) {
            this.title = 'Edit invoice Item';
          } else {
            this.title = 'View invoice Item';
          }
        }
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public save(): void {

    this.onSave();

    this.allors.context.save()
      .subscribe(() => {
        const data: IObject = {
          id: this.invoiceItem.id,
          objectType: this.invoiceItem.objectType,
        };

        this.dialogRef.close(data);
      },
        this.saveService.errorHandler
      );
  }

  public goodSelected(object: any) {
    if (object) {
      this.refreshSerialisedItems(object as Product);
    }
  }

  private refreshSerialisedItems(good: Product): void {

    const { pull, x } = this.metaService;

    const unifiedGoodPullName = `${this.m.UnifiedGood.name}_items`;
    const nonUnifiedGoodPullName = `${this.m.NonUnifiedGood.name}_items`;

    const pulls = [
      pull.NonUnifiedGood({
        name: nonUnifiedGoodPullName,
        object: good.id,
        fetch: {
          Part: {
            SerialisedItems: x
          }
        }
      }),
      pull.UnifiedGood({
        name: unifiedGoodPullName,
        object: good.id,
        fetch: {
          SerialisedItems: x
        }
      })
    ];

    this.allors.context
      .load('Pull', new PullRequest({ pulls }))
      .subscribe((loaded) => {
        const serialisedItems1 = loaded.collections[unifiedGoodPullName] as SerialisedItem[];
        const serialisedItems2 = loaded.collections[nonUnifiedGoodPullName] as SerialisedItem[];
        const items = serialisedItems1 || serialisedItems2;

        this.serialisedItems = items.filter(v => v.AvailableForSale === true);

        if (this.invoiceItem.Product !== this.previousProduct) {
          this.invoiceItem.SerialisedItem = null;
          this.serialisedItem = null;
          this.previousProduct = this.invoiceItem.Product;
        }

      });
  }

  private onSave() {

    if (this.invoiceItem.InvoiceItemType !== this.productItemType) {
      this.invoiceItem.Quantity = 1;
    }
  }
}
