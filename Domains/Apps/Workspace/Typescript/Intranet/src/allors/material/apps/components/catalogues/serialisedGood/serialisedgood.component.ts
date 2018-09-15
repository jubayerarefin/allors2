import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar, MatTabChangeEvent } from '@angular/material';
import { ActivatedRoute, Router, UrlSegment } from '@angular/router';

import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import 'rxjs/add/observable/combineLatest';

import { isType } from '@angular/core/src/type';
import { forEach } from '@angular/router/src/utils/collection';
import { ErrorService, SearchFactory, Loaded, MediaService, Saved, Scope, WorkspaceService, DataService, x } from '../../../../../angular';
import { Brand, Facility, Good, InternalOrganisation, InventoryItemKind, Invoice, InvoiceItem, Locale, LocalisedText, Model, Organisation, OrganisationRole, Ownership, ProductCategory, ProductFeature, ProductType, SalesInvoice, SerialisedInventoryItem, SerialisedInventoryItemCharacteristic, SerialisedInventoryItemCharacteristicType, SerialisedInventoryItemState, Singleton, SupplierOffering, VatRate, VendorProduct } from '../../../../../domain';
import { Contains, Equals, Fetch, PullRequest, Sort, TreeNode } from '../../../../../framework';
import { FetchFactory, MetaDomain } from '../../../../../meta';
import { StateService } from '../../../services/StateService';
import { Fetcher } from '../../Fetcher';
import { switchMap } from 'rxjs/operators';
import { ElectronicAddress } from '../../../../../domain/generated/ElectronicAddress.g';
import { InventoryItemKind } from '../../../../../domain/generated/InventoryItemKind.g';
import { Ownership } from '../../../../../domain/generated/Ownership.g';
import { SerialisedInventoryItem } from '../../../../../domain/generated/SerialisedInventoryItem.g';
import { UnitOfMeasure } from '../../../../../domain/generated/UnitOfMeasure.g';
import { InvoiceItem } from '../../../../../domain/generated/InvoiceItem.g';

@Component({
  templateUrl: './serialisedgood.component.html',
})
export class SerialisedGoodComponent implements OnInit, OnDestroy {

  public m: MetaDomain;
  public good: Good;

  public title: string;
  public subTitle: string;
  public facility: Facility;
  public locales: Locale[];
  public categories: ProductCategory[];
  public productTypes: ProductType[];
  public manufacturers: Organisation[];
  public suppliers: Organisation[];
  public activeSuppliers: Organisation[];
  public selectedSuppliers: Organisation[];
  public supplierOfferings: SupplierOffering[];
  public brands: Brand[];
  public selectedBrand: Brand;
  public models: Model[];
  public selectedModel: Model;
  public inventoryItemKinds: InventoryItemKind[];
  public inventoryItems: SerialisedInventoryItem[];
  public inventoryItem: SerialisedInventoryItem;
  public vendorProduct: VendorProduct;
  public serialisedInventoryItemStates: SerialisedInventoryItemState[];
  public vatRates: VatRate[];
  public ownerships: Ownership[];
  public invoiceItems: InvoiceItem[];
  public salesInvoice: SalesInvoice;
  public organisations: Organisation[];
  public addBrand = false;
  public addModel = false;
  public scope: Scope;
  public organisationFilter: SearchFactory;

  private subscription: Subscription;
  private refresh$: BehaviorSubject<Date>;

  private fetcher: Fetcher;

  constructor(
    private workspaceService: WorkspaceService,
    private dataService: DataService,
    private errorService: ErrorService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    public mediaService: MediaService,
    private stateService: StateService,
  ) {

    this.scope = this.workspaceService.createScope();
    this.m = this.workspaceService.metaPopulation.metaDomain;
    this.refresh$ = new BehaviorSubject<Date>(undefined);
    this.organisationFilter = new SearchFactory({
      objectType: this.m.Organisation,
      roleTypes: [this.m.Organisation.Name],
    });

    this.fetcher = new Fetcher(this.stateService, this.dataService);
  }

  public ngOnInit(): void {

    const { m, pull } = this.dataService;

    this.subscription = Observable.combineLatest(this.route.url, this.refresh$, this.stateService.internalOrganisationId$)
      .pipe(
        switchMap(([urlSegments, date, internalOrganisationId]) => {

          const id: string = this.route.snapshot.paramMap.get('id');

          // const fetch = new FetchFactory(this.workspaceService.metaPopulation);
          // const query = new QueryFactory(this.workspaceService.metaPopulation);

          // const fetches: Fetch[] = [
          //   this.fetcher.locales,
          //   this.fetcher.internalOrganisation,
          //   fetch.Good({
          //     id,
          //     include: {
          //       PrimaryPhoto: {},
          //       Photos: {},
          //       Product_LocalisedNames: {
          //         Localised_Locale: {}
          //       },
          //     },
          //   }),

          const pulls = [
            this.fetcher.locales,
            this.fetcher.internalOrganisation,
            pull.Good({
              include: {
                PrimaryPhoto: x,
                Photos: x,
                ElectronicDocuments: x,
                LocalisedNames: {
                  Locale: x,
                },
                LocalisedDescriptions: {
                  Locale: x,
                },
                LocalisedComments: {
                  Locale: x,
                },
                ProductCategories: x,
                SuppliedBy: x,
                // InventoryItemKind: x,
                // ManufacturedBy: x,
                // StandardFeatures: x,
              }
            }),
            pull.SerialisedInventoryItem({
              object: id,
              include: {
                Ownership: x,
                SerialisedInventoryItemCharacteristics: {
                  SerialisedInventoryItemCharacteristicType: {
                    UnitOfMeasure: x,
                  },
                  LocalisedValues: {
                    Locale: x,
                  }
                }
              }
            }),
            pull.Good(
              {
                object: id,
                fetch: {
                  SalesInvoiceItemsWhereProduct: x
                }
              }
            ),
            pull.Product(
              {
                object: id,
                fetch: {
                  // TODO:
                  // SupplierOfferingsWhereProduct: x
                }
              }
            ),
            pull.VatRate(),
            pull.Ownership({
              sort: new Sort(m.Ownership.Name),
            }),
            pull.InventoryItemKind({ sort: new Sort(m.InventoryItemKind.Name) }),
            pull.SerialisedInventoryItemState({
              predicate: new Equals({ propertyType: m.SerialisedInventoryItemState.IsActive, value: true }),
              sort: new Sort(m.SerialisedInventoryItemState.Name),
            }),
            pull.ProductCategory({ sort: new Sort(m.ProductCategory.Name) }),
            pull.ProductType({ sort: new Sort(m.ProductType.Name) }
            ),
            pull.Brand({ sort: new Sort(m.Brand.Name) })
          ];

          return this.scope
            .load('Pull', new PullRequest({ pulls }))
            .pipe(
              switchMap((loaded) => {
                this.scope.session.reset();

                this.good = loaded.objects.good as Good;
                this.categories = loaded.collections.productCategories as ProductCategory[];
                this.productTypes = loaded.collections.productTypes as ProductType[];
                this.vatRates = loaded.collections.VatRates as VatRate[];
                this.brands = loaded.collections.brands as Brand[];
                this.ownerships = loaded.collections.ownerships as Ownership[];
                this.inventoryItemKinds = loaded.collections.inventoryItemKinds as InventoryItemKind[];
                this.serialisedInventoryItemStates = loaded.collections.serialisedInventoryItemStates as SerialisedInventoryItemState[];
                this.locales = loaded.collections.locales as Locale[];
                const internalOrganisation = loaded.objects.internalOrganisation as InternalOrganisation;
                this.facility = internalOrganisation.DefaultFacility;
                this.invoiceItems = loaded.collections.invoiceItems as InvoiceItem[];
                this.activeSuppliers = internalOrganisation.ActiveSuppliers as Organisation[];
                this.activeSuppliers = this.activeSuppliers.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));

                const vatRateZero = this.vatRates.find((v: VatRate) => v.Rate === 0);
                const inventoryItemKindSerialised = this.inventoryItemKinds.find((v: InventoryItemKind) => v.UniqueId === '2596e2dd-3f5d-4588-a4a2-167d6fbe3fae');

                if (this.good === undefined) {
                  this.good = this.scope.session.create('Good') as Good;
                  this.good.VatRate = vatRateZero;
                  this.good.Sku = '';

                  this.inventoryItem = this.scope.session.create('SerialisedInventoryItem') as SerialisedInventoryItem;
                  // TODO:
                  // this.good.InventoryItemKind = inventoryItemKindSerialised;
                  // this.inventoryItem.Good = this.good;
                  this.inventoryItem.Facility = this.facility;

                  this.vendorProduct = this.scope.session.create('VendorProduct') as VendorProduct;
                  this.vendorProduct.Product = this.good;
                  this.vendorProduct.InternalOrganisation = internalOrganisation;

                } else {
                  this.suppliers = this.good.SuppliedBy as Organisation[];
                  this.selectedSuppliers = this.suppliers;
                  this.supplierOfferings = loaded.collections.supplierOfferings as SupplierOffering[];
                  this.inventoryItems = loaded.collections.inventoryItems as SerialisedInventoryItem[];
                  this.inventoryItem = this.inventoryItems[0];

                  // TODO:
                  // this.good.StandardFeatures.forEach((feature: ProductFeature) => {
                  //   if (feature.objectType.name === 'Brand') {
                  //     this.selectedBrand = feature as Brand;
                  //     this.brandSelected(this.selectedBrand);
                  //   }
                  //   if (feature.objectType.name === 'Model') {
                  //     this.selectedModel = feature as Model;
                  //   }
                  // });
                }

                this.title = this.good.Name;
                this.subTitle = 'Serialised';

                const pulls2 = [
                  pull.Organisation({
                    predicate: new Equals({ propertyType: m.Organisation.IsManufacturer, value: true }),
                    sort: new Sort(m.Organisation.PartyName),
                  })
                ];

                if (this.invoiceItems !== undefined && this.invoiceItems.length > 0) {
                  pulls2.push(
                    pull.SalesInvoiceItem({
                      object: this.invoiceItems[0].id,
                      fetch: {
                        SalesInvoiceWhereSalesInvoiceItem: x
                      }
                    })
                  );

                  return this.scope.load('Pull', new PullRequest({ pulls: pulls2 }));
                }
              }));
        })
      )
      .subscribe((loaded) => {
        this.manufacturers = loaded.collections.manufacturers as Organisation[];
        this.salesInvoice = loaded.objects.invoice as SalesInvoice;
      },
        (error: any) => {
          this.errorService.handle(error);
          this.goBack();
        },
      );
  }

  public brandAdded(brand: Brand): void {
    this.brands.push(brand);
    this.selectedBrand = brand;
    this.models = [];
    this.selectedModel = undefined;
  }

  public modelAdded(model: Model): void {
    // TODO:
    // this.selectedBrand.AddModel(model);
    // this.models = this.selectedBrand.Models.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
    this.selectedModel = model;
  }

  public brandSelected(brand: Brand): void {

    const { m, pull } = this.dataService;

    const pulls = [
      pull.Brand({
        object: brand,
        include: {
          // TODO:
          // Models: x,
        }
      }
      )
    ];

    this.scope
      .load('Pull', new PullRequest({ pulls }))
      .subscribe((loaded) => {
        const selectedBrand = loaded.objects.selectedbrand as Brand;
        // TODO:
        // this.models = selectedBrand.Models.sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0));
      },
        (error: Error) => {
          this.errorService.handle(error);
          this.goBack();
        },
      );
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public save(): void {

    this.onSave();

    this.scope
      .save()
      .subscribe((saved: Saved) => {
        this.goBack();
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public update(): void {
    const isNew = this.good.isNew;

    this.onSave();

    this.scope
      .save()
      .subscribe((saved: Saved) => {
        this.snackBar.open('Successfully saved.', 'close', { duration: 5000 });
        if (isNew) {
          this.router.navigate(['/serialisedGood/' + this.good.id]);
        } else {
          this.refresh();
        }
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public goBack(): void {
    window.history.back();
  }

  private onSave() {
    // TODO:
    // this.good.StandardFeatures.forEach((feature: ProductFeature) => {
    //   this.good.RemoveStandardFeature(feature);
    // });

    // if (this.selectedBrand != null) {
    //   this.good.AddStandardFeature(this.selectedBrand);
    // }

    // if (this.selectedModel != null) {
    //   this.good.AddStandardFeature(this.selectedModel);
    // }

    if (this.suppliers !== undefined) {
      const suppliersToDelete = this.suppliers.filter(v => v);

      if (this.selectedSuppliers !== undefined) {
        this.selectedSuppliers.forEach((supplier: Organisation) => {
          const index = suppliersToDelete.indexOf(supplier);
          if (index > -1) {
            suppliersToDelete.splice(index, 1);
          }

          const now = new Date();
          const supplierOffering = this.supplierOfferings.find((v) =>
            v.Supplier === supplier &&
            v.FromDate <= now &&
            (v.ThroughDate === null || v.ThroughDate >= now));

          if (supplierOffering === undefined) {
            this.supplierOfferings.push(this.newSupplierOffering(supplier, this.good));
          } else {
            supplierOffering.ThroughDate = null;
          }
        });
      }

      if (suppliersToDelete !== undefined) {
        suppliersToDelete.forEach((supplier: Organisation) => {
          const now = new Date();
          const supplierOffering = this.supplierOfferings.find((v) =>
            v.Supplier === supplier &&
            v.FromDate <= now &&
            (v.ThroughDate === null || v.ThroughDate >= now));

          if (supplierOffering !== undefined) {
            supplierOffering.ThroughDate = new Date();
          }
        });
      }
    }
  }

  private newSupplierOffering(supplier: Organisation, good: Good): SupplierOffering {
    const supplierOffering = this.scope.session.create('SupplierOffering') as SupplierOffering;
    supplierOffering.Supplier = supplier;
    // TODO:
    // supplierOffering.Product = good;
    return supplierOffering;
  }
}
