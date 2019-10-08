import * as moment from 'moment';

import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Subscription } from 'rxjs';
import { switchMap, filter } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { ContextService, NavigationService, PanelService, RefreshService, MetaService, Saved, FetcherService, TestScope } from '../../../../../../angular';
import { Enumeration, InternalOrganisation, Locale, Organisation, SerialisedItem, Part, SupplierRelationship, SerialisedInventoryItem, Facility, ProductCategory } from '../../../../../../domain';
import { SaveService, FiltersService } from '../../../../../../material';
import { Equals, PullRequest, Sort } from '../../../../../../framework';
import { Meta } from '../../../../../../meta';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'serialiseditem-overview-detail',
  templateUrl: './serialiseditem-overview-detail.component.html',
  providers: [PanelService, ContextService]
})
export class SerialisedItemOverviewDetailComponent extends TestScope implements OnInit, OnDestroy {

  readonly m: Meta;

  serialisedItem: SerialisedItem;

  internalOrganisation: InternalOrganisation;
  locales: Locale[];
  serialisedItemStates: Enumeration[];
  ownerships: Enumeration[];
  parts: Part[];
  part: Part;
  currentSuppliers: Organisation[];
  currentFacility: Facility;

  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    @Self() public panel: PanelService,
    public filtersService: FiltersService,
    private metaService: MetaService,
    public refreshService: RefreshService,
    public navigationService: NavigationService,
    private saveService: SaveService,
    private snackBar: MatSnackBar,
    private fetcher: FetcherService,
  ) {
    super();

    this.m = this.metaService.m;

    panel.name = 'detail';
    panel.title = 'Serialised Asset data';
    panel.icon = 'business';
    panel.expandable = true;

    // Minimized
    const pullName = `${this.panel.name}_${this.m.SerialisedItem.name}`;

    panel.onPull = (pulls) => {

      this.serialisedItem = undefined;

      if (this.panel.isCollapsed) {
        const { pull } = this.metaService;
        const id = this.panel.manager.id;

        pulls.push(
          pull.SerialisedItem({
            name: pullName,
            object: id,
          })
        );
      }
    };

    panel.onPulled = (loaded) => {
      if (this.panel.isCollapsed) {
        this.serialisedItem = loaded.objects[pullName] as SerialisedItem;
      }
    };
  }

  public ngOnInit(): void {

    // Maximized
    this.subscription = this.panel.manager.on$
      .pipe(
        filter(() => {
          return this.panel.isExpanded;
        }),
        switchMap(() => {

          this.serialisedItem = undefined;

          const { m, pull, x } = this.metaService;
          const id = this.panel.manager.id;

          const pulls = [
            pull.SerialisedItem({
              object: id,
              include: {
                SerialisedItemState: x,
                SerialisedItemCharacteristics: {
                  SerialisedItemCharacteristicType: {
                    UnitOfMeasure: x
                  }
                },
                Ownership: x,
                OwnedBy: x,
                RentedBy: x,
                PrimaryPhoto: x,
                SecondaryPhotos: x,
                AdditionalPhotos: x,
                PrivatePhotos: x,
                PurchaseOrder: x,
                SuppliedBy: x
              }
            }),
            this.fetcher.internalOrganisation,
            this.fetcher.locales,
            pull.SerialisedItem({
              object: id,
              fetch: {
                PartWhereSerialisedItem: {
                  include: { SerialisedItems: x }
                }
              }
            }),
            pull.SerialisedItem({
              object: id,
              fetch: {
                SerialisedInventoryItemsWhereSerialisedItem: {
                  include: {
                    Facility: x
                  }
                }
              }
            }),
            pull.Part({
              include: { SerialisedItems: x }
            }),
            pull.SupplierRelationship({
              include: {
                Supplier: x
              }
            }),
            pull.SerialisedItemState({
              predicate: new Equals({ propertyType: m.SerialisedItemState.IsActive, value: true }),
              sort: new Sort(m.SerialisedItemState.Name),
            }),
            pull.Ownership({
              predicate: new Equals({ propertyType: m.Ownership.IsActive, value: true }),
              sort: new Sort(m.Ownership.Name),
            }),
          ];

          return this.allors.context.load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.allors.context.reset();

        const now = moment.utc();

        const supplierRelationships = loaded.collections.SupplierRelationships as SupplierRelationship[];
        const currentsupplierRelationships = supplierRelationships.filter(v => moment(v.FromDate).isBefore(now) && (v.ThroughDate === null || moment(v.ThroughDate).isAfter(now)));
        this.currentSuppliers = Array.from(new Set(currentsupplierRelationships.map(v => v.Supplier).sort((a, b) => (a.Name > b.Name) ? 1 : ((b.Name > a.Name) ? -1 : 0))));

        this.serialisedItem = loaded.objects.SerialisedItem as SerialisedItem;
        this.locales = loaded.collections.AdditionalLocales as Locale[];
        this.serialisedItemStates = loaded.collections.SerialisedItemStates as Enumeration[];
        this.ownerships = loaded.collections.Ownerships as Enumeration[];
        this.part = loaded.objects.Part as Part;
        this.parts = loaded.collections.Parts as Part[];

        const serialisedInventoryItems = loaded.collections.SerialisedInventoryItems as SerialisedInventoryItem[];
        const inventoryItem = serialisedInventoryItems.find(v => v.Quantity === 1);
        if (inventoryItem) {
          this.currentFacility = inventoryItem.Facility;
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
        this.panel.toggle();
      },
        this.saveService.errorHandler
      );
  }

  public update(): void {
    const { context } = this.allors;

    this.onSave();

    context
      .save()
      .subscribe(() => {
        this.snackBar.open('Successfully saved.', 'close', { duration: 5000 });
        this.refreshService.refresh();
      },
        this.saveService.errorHandler
      );
  }

  private onSave() {
    this.part.AddSerialisedItem(this.serialisedItem);
  }
}
