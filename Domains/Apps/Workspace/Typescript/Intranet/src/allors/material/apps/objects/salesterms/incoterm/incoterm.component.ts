import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute, Router, UrlSegment } from '@angular/router';

import { BehaviorSubject, Observable, Subscription, combineLatest } from 'rxjs';

import { ErrorService, Saved, ContextService, MetaService } from '../../../../../angular';
import { IncoTermType, SalesOrder, SalesTerm } from '../../../../../domain';
import { Fetch, PullRequest, Sort, TreeNode, Equals } from '../../../../../framework';
import { Meta } from '../../../../../meta';
import { AllorsMaterialDialogService } from '../../../../base/services/dialog';
import { switchMap } from 'rxjs/operators';

@Component({
  templateUrl: './incoterm.component.html',
  providers: [ContextService]
})
export class IncoTermEditComponent implements OnInit, OnDestroy {

  public m: Meta;

  public title = 'Edit Sales Order Incoterm';
  public subTitle: string;
  public order: SalesOrder;
  public salesTerm: SalesTerm;
  public incoTermTypes: IncoTermType[];

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() private allors: ContextService,
    public metaService: MetaService,
    private errorService: ErrorService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogService: AllorsMaterialDialogService) {

    this.m = this.metaService.m;
    this.refresh$ = new BehaviorSubject<Date>(undefined);
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = combineLatest(this.route.url, this.refresh$)
      .pipe(
        switchMap(([urlSegments, date]) => {

          const id: string = this.route.snapshot.paramMap.get('id');
          const termId: string = this.route.snapshot.paramMap.get('termId');

          const pulls = [
            pull.SalesOrder({
              object: id
            }),
            pull.SalesTerm({
              object: id,
              include: {
                TermType: x,
              }
            }),
            pull.IncoTermType({
              predicate: new Equals({ propertyType: m.IncoTermType.IsActive, value: true }),
              sort: [
                new Sort(m.IncoTermType.Name),
              ],
            })
          ];

          return this.allors.context
            .load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.order = loaded.objects.salesOrder as SalesOrder;
        this.salesTerm = loaded.objects.salesTerm as SalesTerm;
        this.incoTermTypes = loaded.collections.incoTermTypes as IncoTermType[];

        if (!this.salesTerm) {
          this.title = 'Add Order Incoterm';
          this.salesTerm = this.allors.context.create('IncoTerm') as SalesTerm;
          this.order.AddSalesTerm(this.salesTerm);
        }
      }, this.errorService.handler);
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public save(): void {

    this.allors.context
      .save()
      .subscribe((saved: Saved) => {
        this.router.navigate(['/orders/salesOrder/' + this.order.id]);
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