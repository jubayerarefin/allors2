import { Observable, BehaviorSubject, Subject, Subscription } from 'rxjs/Rx';
import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { MdSnackBar, MdSnackBarConfig } from '@angular/material';

import { TdLoadingService, TdDialogService, TdMediaService } from '@covalent/core';

import { PullRequest, Query, Predicate, And, Or, Not, Equals, Like, Contains, ContainedIn, TreeNode, Sort, Page } from '../../../../domain';
import { MetaDomain } from '../../../../meta/index';
import { Person, OrganisationContactRelationship } from '../../../../domain';
import { AllorsService, ErrorService, Scope, Loaded, Saved, Invoked } from '../../../../angular';

interface SearchData {
  firstName: string;
  lastName: string;
}

@Component({
  templateUrl: './people.component.html',
})
export class PeopleComponent implements AfterViewInit, OnDestroy {

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;
  private scope: Scope;

  private page$: BehaviorSubject<number>;
  total: number;

  searchForm: FormGroup;

  data: Person[];

  constructor(
    private allors: AllorsService,
    private errorService: ErrorService,
    private formBuilder: FormBuilder,
    private titleService: Title,
    private snackBar: MdSnackBar,
    private router: Router,
    private dialogService: TdDialogService,
    private snackBarService: MdSnackBar) {

    this.scope = new Scope(allors.database, allors.workspace);

    this.searchForm = this.formBuilder.group({
      firstName: [''],
      lastName: [''],
    });

    this.page$ = new BehaviorSubject<number>(50);

    const search$: Observable<SearchData> = this.searchForm.valueChanges
      .debounceTime(400)
      .distinctUntilChanged()
      .startWith({});

    const combined$: Observable<any> = Observable.combineLatest(search$, this.page$)
      .scan(([previousData, previousTake]: [SearchData, number], [data, take]: [SearchData, number]): [SearchData, number] => {
        return [
          data,
          data !== previousData ? 50 : take,
        ];
      }, [] as [SearchData, number]);

    this.subscription = combined$
      .switchMap(([data, take]: [SearchData, number]) => {
        const m: MetaDomain = this.allors.meta;

        const predicate: And = new And();
        const predicates: Predicate[] = predicate.predicates;

        if (data.firstName) {
          const like: string = '%' + data.firstName + '%';
          predicates.push(new Like({ roleType: m.Person.FirstName, value: like }));
        }

        if (data.lastName) {
          const like: string = data.lastName.replace('*', '%') + '%';
          predicates.push(new Like({ roleType: m.Person.LastName, value: like }));
        }

        const query: Query[] = [new Query(
          {
            name: 'people',
            objectType: m.Person,
            predicate: predicate,
            page: new Page({ skip: 0, take: take }),
            include: [
              new TreeNode({ roleType: m.Person.Picture }),
            ],
          })];

        return this.scope.load('Pull', new PullRequest({ query: query }));

      })
      .subscribe((loaded: Loaded) => {

        this.scope.session.reset();

        this.data = loaded.collections.people as Person[];
        this.total = loaded.values.people_total;
      },
      (error: any) => {
        this.errorService.message(error);
        this.goBack();
      });
  }

  more(): void {
    this.page$.next(this.data.length + 50);
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  ngAfterViewInit(): void {
    this.titleService.setTitle('People');
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  refresh(): void {
    this.refresh$.next(new Date());
  }

  delete(person: Person): void {
    this.dialogService
      .openConfirm({ message: 'Are you sure you want to delete this person?' })
      .afterClosed()
      .subscribe((confirm: boolean) => {
        if (confirm) {
          this.scope.invoke(person.Delete)
            .subscribe((invoked: Invoked) => {
              this.snackBar.open('Successfully deleted.', 'close', { duration: 5000 });
              this.refresh();
            },
            (error: Error) => {
              this.errorService.dialog(error);
            });
        }
      });
  }

  onView(person: Person): void {
    this.router.navigate(['/relations/people/' + person.id + '/overview']);
  }
}
