import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Subscription, combineLatest } from 'rxjs';
import { switchMap, scan } from 'rxjs/operators';

import { PullRequest, And, Equals } from '../../../../../framework';
import { AllorsFilterService, ErrorService, MediaService, ContextService, NavigationService, Action, RefreshService, MetaService } from '../../../../../angular';
import { Sorter, TableRow, Table, OverviewService, EditService, DeleteService, StateService } from '../../../..';

import { Catalogue } from '../../../../../domain';

interface Row extends TableRow {
  object: Catalogue;
  name: string;
  description: string;
  scope: string;
}

@Component({
  templateUrl: './catalogue-list.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class CataloguesOverviewComponent implements OnInit, OnDestroy {

  public title = 'Catalogues';

  table: Table<Row>;

  edit: Action;
  delete: Action;

  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    @Self() private filterService: AllorsFilterService,
    public metaService: MetaService,
    public refreshService: RefreshService,
    public overviewService: OverviewService,
    public editService: EditService,
    public deleteService: DeleteService,
    public navigation: NavigationService,
    public mediaService: MediaService,
    private errorService: ErrorService,
    private stateService: StateService,
    titleService: Title,
  ) {
    titleService.setTitle(this.title);

    this.edit = editService.edit();
    this.edit.result.subscribe(() => {
      this.table.selection.clear();
    });

    this.delete = deleteService.delete(allors.context);
    this.delete.result.subscribe(() => {
      this.table.selection.clear();
    });

    this.table = new Table({
      selection: true,
      columns: [
        { name: 'name', sort: true },
        { name: 'description', sort: true },
        { name: 'scope', sort: true }
      ],
      actions: [
        this.edit,
        this.delete
      ],
      defaultAction: this.edit
    });
  }

  ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    const internalOrganisationPredicate = new Equals({ propertyType: m.Catalogue.InternalOrganisation });
    const predicate = new And([
      // new Like({ roleType: m.Person.FirstName, parameter: 'firstName' }),
      internalOrganisationPredicate
    ]);

    this.filterService.init(predicate);

    const sorter = new Sorter(
      {
        name: m.Catalogue.Name,
        description: m.Catalogue.Description,
        scope: m.CatScope.Name,
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
            pull.Catalogue({
              predicate,
              sort: sorter.create(sort),
              include: {
                CatalogueImage: x,
                ProductCategories: x,
                CatScope: x
              },
              arguments: this.filterService.arguments(filterFields),
              skip: pageEvent.pageIndex * pageEvent.pageSize,
              take: pageEvent.pageSize,
            })];

          return this.allors.context.load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.context.reset();

        const objects = loaded.collections.Catalogues as Catalogue[];
        this.table.total = loaded.values.Catalogues_total;
        this.table.data = objects.map((v) => {
          return {
            object: v,
            name: `${v.Name}`,
            description: `${v.Description || ''}`,
            scope: v.CatScope.Name
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
