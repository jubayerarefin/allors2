import { Component, OnDestroy, OnInit, Self, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Subscription, combineLatest } from 'rxjs';

import { ContextService, MetaService, RefreshService, InternalOrganisationId, FetcherService, TestScope } from '../../../../../angular';
import { Part, Organisation, SupplierOffering, RatingType, Ordinal, UnitOfMeasure, Currency, Settings } from '../../../../../domain';
import { PullRequest, Sort, IObject } from '../../../../../framework';
import { Meta } from '../../../../../meta';
import { switchMap, map } from 'rxjs/operators';
import { ObjectData } from '../../../../../material/base/services/object';
import { SaveService } from '../../../../../../allors/material';


@Component({
  templateUrl: './supplieroffering-edit.component.html',
  providers: [ContextService]
})
export class SupplierOfferingEditComponent extends TestScope implements OnInit, OnDestroy {

  readonly m: Meta;

  supplierOffering: SupplierOffering;
  part: Part;
  ratingTypes: RatingType[];
  preferences: Ordinal[];
  activeSuppliers: Organisation[];
  unitsOfMeasure: UnitOfMeasure[];
  currencies: Currency[];
  settings: Settings;

  private subscription: Subscription;
  title: string;

  constructor(
    @Self() private allors: ContextService,
    @Inject(MAT_DIALOG_DATA) public data: ObjectData,
    public dialogRef: MatDialogRef<SupplierOfferingEditComponent>,
    public metaService: MetaService,
    public refreshService: RefreshService,
    private saveService: SaveService,
    private fetcher: FetcherService,
    private internalOrganisationId: InternalOrganisationId,
  ) {
    super();

    this.m = this.metaService.m;
  }

  public ngOnInit(): void {

    const { pull, x, m } = this.metaService;

    this.subscription = combineLatest(this.refreshService.refresh$, this.internalOrganisationId.observable$)
      .pipe(
        switchMap(([]) => {

          const isCreate = this.data.id === undefined;

          let pulls = [
            this.fetcher.Settings,
            pull.RatingType({ sort: new Sort(m.RateType.Name) }),
            pull.Ordinal({ sort: new Sort(m.Ordinal.Name) }),
            pull.UnitOfMeasure({ sort: new Sort(m.UnitOfMeasure.Name) }),
            pull.Currency({ sort: new Sort(m.Currency.Name) }),
          ];

          if (isCreate) {
            pulls = [
              ...pulls,
              pull.Part({
                object: this.data.associationId,
                include: {
                  SuppliedBy: x
                }
              }),
            ];
          }

          if (!isCreate) {
            pulls = [
              ...pulls,
              pull.SupplierOffering({
                object: this.data.id,
                include: {
                  Part: x,
                  Rating: x,
                  Preference: x,
                  Supplier: x,
                  Currency: x,
                  UnitOfMeasure: x
                }
              }),
            ];
          }

          return this.allors.context
            .load('Pull', new PullRequest({ pulls }))
            .pipe(
              map((loaded) => ({ loaded, isCreate }))
            );
        })
      )
      .subscribe(({ loaded, isCreate }) => {

        this.allors.context.reset();

        this.ratingTypes = loaded.collections.RatingTypes as RatingType[];
        this.preferences = loaded.collections.Ordinals as Ordinal[];
        this.unitsOfMeasure = loaded.collections.UnitsOfMeasure as UnitOfMeasure[];
        this.currencies = loaded.collections.Currencies as Currency[];
        this.settings = loaded.objects.Settings as Settings;

        if (isCreate) {
          this.title = 'Add supplier offering';

          this.supplierOffering = this.allors.context.create('SupplierOffering') as SupplierOffering;
          this.part = loaded.objects.Part as Part;
          this.supplierOffering.Part = this.part;
          this.supplierOffering.Currency = this.settings.PreferredCurrency;

        } else {

          this.supplierOffering = loaded.objects.SupplierOffering as SupplierOffering;
          this.part = this.supplierOffering.Part;

          if (this.supplierOffering.CanWritePrice) {
            this.title = 'Edit supplier offering';
          } else {
            this.title = 'View supplier offering';
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

    this.allors.context.save()
      .subscribe(() => {
        const data: IObject = {
          id: this.supplierOffering.id,
          objectType: this.supplierOffering.objectType,
        };

        this.dialogRef.close(data);
      },
        this.saveService.errorHandler
      );
  }
}
