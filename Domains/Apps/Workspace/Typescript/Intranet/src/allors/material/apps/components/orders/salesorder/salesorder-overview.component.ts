import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatSnackBar } from '@angular/material';
import { ActivatedRoute, Router, UrlSegment } from '@angular/router';

import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ErrorService, Invoked, Loaded, MediaService, PdfService, Saved, Scope, WorkspaceService, DatabaseService, DataService, x } from '../../../../../angular';
import { BillingProcess, Good, ProductQuote, SalesInvoice, SalesOrder, SalesOrderItem, SalesTerm, SerialisedInventoryItemState } from '../../../../../domain';
import { Fetch, PullRequest, TreeNode, Sort, Equals } from '../../../../../framework';
import { MetaDomain } from '../../../../../meta';
import { AllorsMaterialDialogService } from '../../../../base/services/dialog';
import { Product } from '../../../../../domain/generated/Product.g';
import { SalesTerm } from '../../../../../domain/generated/SalesTerm.g';
import { TermType } from '../../../../../domain/generated/TermType.g';
import { PostalBoundary } from '../../../../../domain/generated/PostalBoundary.g';
import { Country } from '../../../../../domain/generated/Country.g';
import { PostalAddress } from '../../../../../domain/generated/PostalAddress.g';

@Component({
  templateUrl: './salesorder-overview.component.html',
})
export class SalesOrderOverviewComponent implements OnInit, OnDestroy {

  public m: MetaDomain;
  public title = 'Sales Order Overview';
  public quote: ProductQuote;
  public order: SalesOrder;
  public orderItems: SalesOrderItem[] = [];
  public goods: Good[] = [];
  public salesInvoice: SalesInvoice;
  public billingProcesses: BillingProcess[];
  public billingForOrderItems: BillingProcess;
  public selectedSerialisedInventoryState: string;
  public inventoryItemStates: SerialisedInventoryItemState[];
  private subscription: Subscription;
  private scope: Scope;
  private refresh$: BehaviorSubject<Date>;

  constructor(
    private workspaceService: WorkspaceService,
    private dataService: DataService,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    public mediaService: MediaService,
    public pdfService: PdfService,
    private dialogService: AllorsMaterialDialogService) {

    this.scope = this.workspaceService.createScope();
    this.m = this.workspaceService.metaPopulation.metaDomain;
    this.refresh$ = new BehaviorSubject<Date>(undefined);
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public save(): void {

    this.scope
      .save()
      .subscribe((saved: Saved) => {
        this.snackBar.open('items saved', 'close', { duration: 1000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public ngOnInit(): void {

    const { m, pull } = this.dataService;

    this.subscription = Observable.combineLatest(this.route.url, this.refresh$)
      .pipe(
        switchMap(([urlSegments, date]) => {
          const id: string = this.route.snapshot.paramMap.get('id');

          const pulls = [
            pull.SalesOrder({
              object: id,
              include: {
                SalesOrderItems: {
                  Product: x,
                  InvoiceItemType: x,
                  SalesOrderItemState: x,
                  SalesOrderItemShipmentState: x,
                  SalesOrderItemPaymentState: x,
                  SalesOrderItemInvoiceState: x,
                },
                SalesTerms: {
                  TermType: x,
                },
                BillToCustomer: x,
                BillToContactPerson: x,
                ShipToCustomer: x,
                ShipToContactPerson: x,
                ShipToEndCustomer: x,
                ShipToEndCustomerContactPerson: x,
                BillToEndCustomer: x,
                BillToEndCustomerContactPerson: x,
                SalesOrderState: x,
                SalesOrderShipmentState: x,
                SalesOrderInvoiceState: x,
                SalesOrderPaymentState: x,
                CreatedBy: x,
                LastModifiedBy: x,
                Quote: x,
                ShipToAddress: {
                  PostalBoundary: {
                    Country: x,
                  }
                },
                BillToEndCustomerContactMechanism: {
                  PostalAddress_PostalBoundary: {
                    Country: x,
                  }
                },
                ShipToEndCustomerAddress: {
                  PostalBoundary: {
                    Country: x,
                  }
                },
                BillToContactMechanism: {
                  PostalAddress_PostalBoundary: {
                    Country: x,
                  }
                }
              }
            }),
            pull.Good({ sort: new Sort(m.Good.Name) }),
            pull.BillingProcess({ sort: new Sort(m.BillingProcess.Name) }),
            pull.SerialisedInventoryItemState({
              predicate: new Equals({ propertyType: m.SerialisedInventoryItemState.IsActive, value: true }),
              sort: new Sort(m.SerialisedInventoryItemState.Name)
            })
          ];

          const salesInvoiceFetch = pull.SalesOrder({
            object: id,
            fetch: { SalesInvoicesWhereSalesOrder: x }
          });

          if (id != null) {
            pulls.push(salesInvoiceFetch);
          }

          return this.scope
            .load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.scope.session.reset();
        this.goods = loaded.collections.goods as Good[];
        this.order = loaded.objects.order as SalesOrder;
        this.salesInvoice = loaded.objects.salesInvoice as SalesInvoice;
        this.inventoryItemStates = loaded.collections.serialisedInventoryItemStates as SerialisedInventoryItemState[];
        this.billingProcesses = loaded.collections.billingProcesses as BillingProcess[];
        this.billingForOrderItems = this.billingProcesses.find((v: BillingProcess) => v.UniqueId.toUpperCase() === 'AB01CCC2-6480-4FC0-B20E-265AFD41FAE2');

        if (this.order) {
          this.orderItems = this.order.SalesOrderItems;
        }
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

  public goBack(): void {
    window.history.back();
  }

  public print() {
    this.pdfService.display(this.order);
  }

  public approve(): void {
    this.scope.invoke(this.order.Approve)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully approved.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public cancel(): void {
    this.scope.invoke(this.order.Reject)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully cancelled.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public reject(): void {
    this.scope.invoke(this.order.Reject)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully rejected.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public hold(): void {
    this.scope.invoke(this.order.Hold)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully put on hold.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public continue(): void {
    this.scope.invoke(this.order.Continue)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully removed from hold.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public confirm(): void {
    this.scope.invoke(this.order.Confirm)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully confirmed.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public finish(): void {
    this.scope.invoke(this.order.Continue)
      .subscribe((invoked: Invoked) => {
        this.refresh();
        this.snackBar.open('Successfully finished.', 'close', { duration: 5000 });
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public cancelOrderItem(orderItem: SalesOrderItem): void {
    this.scope.invoke(orderItem.Cancel)
      .subscribe((invoked: Invoked) => {
        this.snackBar.open('Order Item successfully cancelled.', 'close', { duration: 5000 });
        this.refresh();
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public rejectOrderItem(orderItem: SalesOrderItem): void {
    this.scope.invoke(orderItem.Reject)
      .subscribe((invoked: Invoked) => {
        this.snackBar.open('Order Item successfully rejected.', 'close', { duration: 5000 });
        this.refresh();
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public deleteOrderItem(orderItem: SalesOrderItem): void {
    this.dialogService
      .confirm({ message: 'Are you sure you want to delete this item?' })
      .subscribe((confirm: boolean) => {
        if (confirm) {
          this.scope.invoke(orderItem.Delete)
            .subscribe((invoked: Invoked) => {
              this.snackBar.open('Successfully deleted.', 'close', { duration: 5000 });
              this.refresh();
            },
              (error: Error) => {
                this.errorService.handle(error);
              });
        }
      });
  }

  public deleteSalesTerm(salesTerm: SalesTerm): void {
    this.dialogService
      .confirm({ message: 'Are you sure you want to delete this order term?' })
      .subscribe((confirm: boolean) => {
        if (confirm) {
          this.scope.invoke(salesTerm.Delete)
            .subscribe((invoked: Invoked) => {
              this.snackBar.open('Successfully deleted.', 'close', { duration: 5000 });
              this.refresh();
            },
              (error: Error) => {
                this.errorService.handle(error);
              });
        }
      });
  }

  public ship(): void {
    this.scope.invoke(this.order.Ship)
      .subscribe((invoked: Invoked) => {
        this.goBack();
        this.snackBar.open('Customer Shipment successfully created.', 'close', { duration: 5000 });
        this.refresh();
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public createInvoice(): void {
    this.scope.invoke(this.order.Invoice)
      .subscribe((invoked: Invoked) => {
        this.goBack();
        this.snackBar.open('Invoice successfully created.', 'close', { duration: 5000 });
        this.gotoInvoice();
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public gotoInvoice(): void {

    const { m, pull } = this.dataService;

    const pulls = [
      pull.SalesOrder({
        object: this.order,
        fetch: { SalesInvoicesWhereSalesOrder: x }
      })
    ];

    this.scope.load('Pull', new PullRequest({ pulls }))
      .subscribe((loaded) => {
        const invoices = loaded.collections.invoices as SalesInvoice[];
        if (invoices.length === 1) {
          this.router.navigate(['/accountsreceivable/invoice/' + invoices[0].id]);
        }
      },
        (error: any) => {
          this.errorService.handle(error);
          this.goBack();
        });
  }
}
