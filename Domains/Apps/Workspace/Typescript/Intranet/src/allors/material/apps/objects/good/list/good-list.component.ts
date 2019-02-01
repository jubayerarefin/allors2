import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Subscription, combineLatest } from 'rxjs';
import { switchMap, scan } from 'rxjs/operators';
import * as moment from 'moment';

import { PullRequest, And, Like, Equals, ContainedIn, Filter, Contains, Exists } from '../../../../../framework';
import { AllorsFilterService, ErrorService, MediaService, ContextService, NavigationService, Action, RefreshService, MetaService, SearchFactory } from '../../../../../angular';
import { Sorter, TableRow, Table, OverviewService, DeleteService, StateService } from '../../../..';

import { Good, ProductCategory, IGoodIdentification } from '../../../../../domain';

import { ObjectService } from '../../../../../material/base/services/object';


interface Row extends TableRow {
  object: Good;
  name: string;
  categories: string;
  qoh: number;
}

@Component({
  templateUrl: './good-list.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class GoodListComponent implements OnInit, OnDestroy {

  public title = 'Products';

  table: Table<Row>;

  delete: Action;

  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    @Self() private filterService: AllorsFilterService,
    public metaService: MetaService,
    public factoryService: ObjectService,
    public refreshService: RefreshService,
    public overviewService: OverviewService,
    public deleteService: DeleteService,
    public navigation: NavigationService,
    public mediaService: MediaService,
    private errorService: ErrorService,
    private stateService: StateService,
    titleService: Title) {

    titleService.setTitle(this.title);

    this.delete = deleteService.delete(allors.context);
    this.delete.result.subscribe((v) => {
      this.table.selection.clear();
    });

    this.table = new Table({
      selection: true,
      columns: [
        { name: 'name', sort: true },
        { name: 'categories' },
        { name: 'qoh' },
      ],
      actions: [
        overviewService.overview(),
        this.delete
      ],
      defaultAction: overviewService.overview(),
      pageSize: 50,
    });
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    const internalOrganisationPredicate = new Equals({ propertyType: m.VendorProduct.InternalOrganisation });

    const predicate = new And([
      new Like({ roleType: m.Good.Name, parameter: 'name' }),
      new Like({ roleType: m.Good.Keywords, parameter: 'keyword' }),
      new Contains({ propertyType: m.Good.ProductCategoriesWhereProduct, parameter: 'category' }),
      new Contains({ propertyType: m.Good.GoodIdentifications, parameter: 'identification' }),
      new Exists({ parameter: 'discontinued' }),
    ]);

    const categorySearch = new SearchFactory({
      objectType: m.ProductCategory,
      roleTypes: [m.ProductCategory.Name],
    });

    const idSearch = new SearchFactory({
      objectType: m.IGoodIdentification,
      roleTypes: [m.IGoodIdentification.Identification],
    });

    this.filterService.init(predicate,
      {
        category: { search: categorySearch, display: (v: ProductCategory) => v.Name },
        identification: { search: idSearch, display: (v: IGoodIdentification) => v.Identification },
        discontinued: { exist: m.Good.SalesDiscontinuationDate },
      });

    const sorter = new Sorter(
      {
        name: [m.Good.Name],
      }
    );

    this.subscription = combineLatest(this.refreshService.refresh$, this.filterService.filterFields$, this.table.sort$, this.table.pager$, this.stateService.internalOrganisationId$)
      .pipe(
        scan(([previousRefresh, previousFilterFields], [refresh, filterFields, sort, pageEvent, internalOrganisationId]) => {
          return [
            refresh,
            filterFields,
            sort,
            (previousRefresh !== refresh || filterFields !== previousFilterFields) ? Object.assign({ pageIndex: 0 }, pageEvent) : pageEvent,
            internalOrganisationId
          ];
        }, []),
        switchMap(([, filterFields, sort, pageEvent, internalOrganisationId]) => {

          internalOrganisationPredicate.object = internalOrganisationId;

          const pulls = [
            pull.Good({
              predicate,
              sort: sorter.create(sort),
              include: {
                Part: x,
                PrimaryPhoto: x,
              },
              arguments: this.filterService.arguments(filterFields),
              skip: pageEvent.pageIndex * pageEvent.pageSize,
              take: pageEvent.pageSize,
            }),
            pull.Good({
              predicate,
              sort: sorter.create(sort),
              fetch: {
                ProductCategoriesWhereProduct: {
                  include: {
                    Products: x,
                  }
                },
              }
            })
          ];

          return this.allors.context.load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.context.reset();

        const goods = loaded.collections.Goods as Good[];
        const productCategories = loaded.collections.ProductCategories as ProductCategory[];

        this.table.total = loaded.values.Goods_total;
        this.table.data = goods.map((v) => {
          return {
            object: v,
            name: v.Name,
            categories: productCategories.filter(w => w.Products.includes(v)).map((w) => w.Name).join(', '),
            qoh: v.Part.QuantityOnHand
          } as Row;
        });
      }, this.errorService.handler);
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
