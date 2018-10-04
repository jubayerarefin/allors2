import { Component, OnDestroy, OnInit, ViewChild, Self } from '@angular/core';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { MatSnackBar, MatTableDataSource, MatSort, MatDialog, Sort } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';

import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ErrorService, Invoked, MediaService, x, Allors } from '../../../../../../angular';
import { PullRequest, SessionObject, Filter, And, Predicate, Like, Sort as AllorsSort } from '../../../../../../framework';
import { AllorsMaterialDialogService } from '../../../../../base/services/dialog';

import { Person } from '../../../../../../domain';
import { PersonAddComponent } from '../add/person-add.module';
import { AllorsFilterService, FilterFieldPredicate, FilterFieldType } from '../../../../../../angular/base/filter';

interface Row {
  person: Person;
  name: string;
  email: string;
  phone: string;
  lastModifiedDate: Date;
}

@Component({
  templateUrl: './person-list.component.html',
  providers: [Allors, AllorsFilterService]
})
export class PersonListComponent implements OnInit, OnDestroy {

  public title = 'People';

  public displayedColumns = ['select', 'name', 'email', 'phone', 'lastModifiedDate', 'menu'];
  public dataSource = new MatTableDataSource<Row>();
  public selection = new SelectionModel<Row>(true, []);

  public data: Row[];

  public sort$: BehaviorSubject<Sort>;
  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() private allors: Allors,
    @Self() private filterService: AllorsFilterService,
    public mediaService: MediaService,
    public router: Router,
    private errorService: ErrorService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private dialogService: AllorsMaterialDialogService,
    private location: Location,
    titleService: Title) {

    titleService.setTitle(this.title);

    this.sort$ = new BehaviorSubject<Sort>(undefined);
    this.refresh$ = new BehaviorSubject<Date>(undefined);

    filterService.filterFieldDefinitions.push(
      {
        name: 'First Name',
        predicate: FilterFieldPredicate.StartsWith,
        type: FilterFieldType.String
      },
      {
        name: 'Last Name',
        predicate: FilterFieldPredicate.StartsWith,
        type: FilterFieldType.String
      }
    );
  }

  public ngOnInit(): void {

    const { m, pull, scope } = this.allors;

    this.subscription = combineLatest(this.filterService.filterFields$, this.sort$, this.refresh$)
      .pipe(
        switchMap(([filterFields, sort]) => {

          const predicates = filterFields.map(v => {
            if (v.definition === this.filterService.filterFieldDefinitions[0]) {
              return new Like(m.Person.FirstName, v.value + '%');
            }

            if (v.definition === this.filterService.filterFieldDefinitions[1]) {
              return new Like(m.Person.LastName, v.value + '%');
            }

          });

          let sorting: AllorsSort[];
          if (sort) {
            const descending = sort.direction === 'desc';

            switch (sort.active) {
              case 'name':
                sorting = [
                  new AllorsSort({
                    roleType: m.Person.FirstName,
                    descending
                  }),
                  new AllorsSort({
                    roleType: m.Person.LastName,
                    descending
                  }),
                ];

                break;
            }
          }

          const pulls = [
            pull.Person({
              predicate: predicates.length > 0 ? new And({ operands: predicates }) : undefined,
              sort: sorting,
              include: {
                Salutation: x,
                Picture: x,
                GeneralPhoneNumber: x,
                GeneralEmail: x,
              },
            })];

          return scope.load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        scope.session.reset();
        const people = loaded.collections.People as Person[];
        this.data = people.map((v) => {
          return {
            person: v,
            name: v.displayName,
            email: v.displayEmail,
            phone: v.displayPhone,
            lastModifiedDate: v.LastModifiedDate,
          } as Row;
        });

        this.dataSource.data = this.data;
      },
        (error: any) => {
          this.errorService.handle(error);
          this.goBack();
        });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public get hasSelection() {
    return !this.selection.isEmpty();
  }

  public get hasDeleteSelection() {
    return this.selectedPeople.filter((v) => v.CanExecuteDelete).length > 0;
  }

  public get selectedPeople() {
    return this.selection.selected.map(v => v.person);
  }

  public isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  public masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  public goBack(): void {
    this.location.back();
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public delete(person: Person | Person[]): void {

    const { scope } = this.allors;

    const people = person instanceof SessionObject ? [person as Person] : person instanceof Array ? person : [];
    const methods = people.filter((v) => v.CanExecuteDelete).map((v) => v.Delete);

    if (methods.length > 0) {
      this.dialogService
        .confirm(
          methods.length === 1 ?
            { message: 'Are you sure you want to delete this person?' } :
            { message: 'Are you sure you want to delete these people?' })
        .subscribe((confirm: boolean) => {
          if (confirm) {
            scope.invoke(methods)
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
  }

  public onView(person: Person): void {
    this.router.navigate(['/relations/person/' + person.id]);
  }

  public addNew() {
    const dialogRef = this.dialog.open(PersonAddComponent, {
      autoFocus: false,
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(result => {
      this.refresh();
    });
  }
}