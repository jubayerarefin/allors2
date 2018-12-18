import { Component, OnDestroy, Input, Output, EventEmitter, OnInit, Self } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';

import { ErrorService, ContextService, NavigationActivatedRoute, NavigationService, Action, ActionTarget, MetaService } from '../../../../../angular';
import { Part, InventoryItem, InventoryItemKind, NonSerialisedInventoryItem, SerialisedInventoryItem } from '../../../../../domain';
import { PullRequest, ObjectType } from '../../../../../framework';
import { Meta } from '../../../../../meta';
import { StateService } from '../../../services/state';
import { switchMap, map } from 'rxjs/operators';
import { TableRow, Table } from 'src/allors/material/base/components/table';
import { NavigateService } from 'src/allors/material/base/actions';

interface Row extends TableRow {
  object: InventoryItem;
  name: string;
  state: string;
  qoh: string;
  atp: string;
}

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'nonserialisedinventoryitem-embed',
  templateUrl: './nonserialisedinventoryitem-embed.component.html',
  providers: [ContextService]
})
export class NonSerialisedInventoryComponent implements OnInit, OnDestroy {

  @Input() part: Part;

  @Output() add: EventEmitter<ObjectType> = new EventEmitter<ObjectType>();

  @Output() edit: EventEmitter<ObjectType> = new EventEmitter<ObjectType>();

  @Output() delete: EventEmitter<InventoryItem> = new EventEmitter<InventoryItem>();

  title = 'Inventory';

  table: Table<Row>;
  receiveInventory: Action;

  m: Meta;

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;
  inventoryItems: InventoryItem[];

  constructor(
    @Self() public allors: ContextService,
    public metaService: MetaService,
    public navigateService: NavigateService,
    public navigation: NavigationService,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    private stateService: StateService,
  ) {
    this.m = this.metaService.m;
    this.refresh$ = new BehaviorSubject<Date>(undefined);

    this.table = new Table({
      selection: false,
      columns: [
        { name: 'name', sort: true },
        { name: 'state', sort: true },
        'uom',
        'qoh',
        'atp',
      ],
      actions: [
        {
          name: () => 'Add transaction',
          description: () => '',
          disabled: () => false,
          execute: (target: ActionTarget) => {
            if (!Array.isArray(target)) {
              this.navigateService.navigationService.add(this.m.InventoryItemTransaction, target, this.part);
            }
          },
          result: null
        }
      ],
    });
  }

  public ngOnInit(): void {

    const { pull, x } = this.metaService;

    this.subscription = combineLatest(this.route.url, this.refresh$, this.stateService.internalOrganisationId$)
      .pipe(
        switchMap(([urlSegments, date, internalOrganisationId]) => {

          const navRoute = new NavigationActivatedRoute(this.route);
          const id = navRoute.id();

          const pulls = [
            pull.Part({
              object: id,
              fetch: {
                InventoryItemsWherePart: {
                  include: {
                    Facility: x,
                    UnitOfMeasure: x,
                    NonSerialisedInventoryItem_NonSerialisedInventoryItemState: x
                  }
                }
              },
            })
          ];

          return this.allors.context
            .load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.allors.context.reset();

        const inventoryItems = loaded.collections.InventoryItems as NonSerialisedInventoryItem[];

        this.table.data = inventoryItems.map((v) => {
          return {
            object: v,
            name: v.Facility.Name,
            state: v.NonSerialisedInventoryItemState.Name,
            uom: v.UnitOfMeasure.Abbreviation || v.UnitOfMeasure.Name,
            qoh: v.QuantityOnHand.toString(),
            atp: v.AvailableToPromise.toString(),
          } as Row;
        });
      },
        (error: any) => {
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

    this.allors.context
      .save()
      .subscribe(() => {
        this.goBack();
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
}