import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { ActivatedRoute, UrlSegment } from '@angular/router';

import { BehaviorSubject, Observable, Subscription, combineLatest } from 'rxjs';

import { ErrorService, Saved, Scope, WorkspaceService, x, Allors } from '../../../../../angular';
import { ContactMechanism, InternalOrganisation, Organisation, OrganisationContactRelationship, Party, PartyContactMechanism, Person, Priority, Singleton, WorkEffortPartyAssignment, WorkEffortPurpose, WorkEffortState, WorkTask } from '../../../../../domain';
import { Fetch, PullRequest, TreeNode, Sort, Equals } from '../../../../../framework';
import { MetaDomain } from '../../../../../meta';
import { StateService } from '../../../services/StateService';
import { Fetcher } from '../../Fetcher';
import { switchMap } from 'rxjs/operators';

@Component({
  templateUrl: './worktask.component.html',
  providers: [Allors]
})
export class WorkTaskEditComponent implements OnInit, OnDestroy {
  public title = 'Work Task';
  public subTitle: string;

  public m: MetaDomain;

  public workTask: WorkTask;

  public workEffortStates: WorkEffortState[];
  public priorities: Priority[];
  public workEffortPurposes: WorkEffortPurpose[];
  public singleton: Singleton;
  public employees: Person[];
  public workEffortPartyAssignments: WorkEffortPartyAssignment[];
  public assignees: Party[] = [];
  public existingAssignees: Party[] = [];
  public contactMechanisms: ContactMechanism[];
  public contacts: Person[];
  public addContactPerson = false;
  public addContactMechanism: boolean;
  public scope: Scope;

  private subscription: Subscription;
  private refresh$: BehaviorSubject<Date>;
  private fetcher: Fetcher;

  constructor(
    @Self() private allors: Allors,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    public stateService: StateService,
  ) {
    this.refresh$ = new BehaviorSubject<Date>(undefined);

    this.m = this.allors.m;
    this.fetcher = new Fetcher(this.stateService, this.allors.pull);
  }

  public ngOnInit(): void {

    const { m, pull, scope } = this.allors;

    this.subscription = combineLatest(
      this.route.url,
      this.refresh$,
      this.stateService.internalOrganisationId$,
    )
      .pipe(
        switchMap(([urlSegments, date, internalOrganisationId]) => {
          const id: string = this.route.snapshot.paramMap.get('id');

          const pulls = [
            this.fetcher.internalOrganisation,
            pull.WorkTask({
              object: id,
              include: {
                FullfillContactMechanism: x,
                ContactPerson: x,
                CreatedBy: x,
              }
            }),
            pull.WorkEffortState({
              sort: new Sort(m.WorkEffortState.Name)
            }),
            pull.Priority({
              predicate: new Equals({ propertyType: m.Priority.IsActive, value: true }),
              sort: new Sort(m.Priority.Name),
            }),
            pull.WorkEffortPurpose({
              predicate: new Equals({ propertyType: this.m.WorkEffortPurpose.IsActive, value: true }),
              sort: new Sort(m.WorkEffortPurpose.Name),
            })
          ];

          return scope
            .load('Pull', new PullRequest({ pulls }))
            .pipe(
              switchMap((loaded) => {
                this.subTitle = 'edit work task';
                this.workTask = loaded.objects.worktask as WorkTask;

                const addMode: boolean = !this.workTask;

                if (addMode) {
                  this.subTitle = 'add a new work task';
                  this.workTask = scope.session.create('WorkTask') as WorkTask;
                }

                this.workEffortStates = loaded.collections
                  .workEffortStates as WorkEffortState[];
                this.priorities = loaded.collections.priorities as Priority[];
                this.workEffortPurposes = loaded.collections
                  .workEffortPurposes as WorkEffortPurpose[];
                const internalOrganisation: InternalOrganisation = loaded.objects
                  .internalOrganisation as InternalOrganisation;
                this.employees = internalOrganisation.ActiveEmployees;

                const assignmentsFetch = [
                  pull.WorkTask({
                    object: id,
                    fetch: {
                      WorkEffortPartyAssignmentsWhereAssignment: x,
                    }
                  })
                ];

                if (this.workTask.Customer) {
                  this.updateCustomer(this.workTask.Customer);
                }

                if (addMode) {
                  return scope.load('Pull', new PullRequest({}));
                } else {
                  return scope.load(
                    'Pull',
                    new PullRequest({ pulls: assignmentsFetch }),
                  );
                }
              })
            );
        })
      )
      .subscribe(
        (loaded) => {
          this.workEffortPartyAssignments = loaded.collections
            .workEffortAssignments as WorkEffortPartyAssignment[];

          if (this.workEffortPartyAssignments) {
            this.assignees = this.workEffortPartyAssignments.map(
              (v: WorkEffortPartyAssignment) => v.Party,
            );
          }

          this.existingAssignees = this.assignees;
        },
        (error: any) => {
          this.errorService.handle(error);
          this.goBack();
        },
      );
  }

  public customerSelected(customer: Party) {
    this.updateCustomer(customer);
  }

  public contactPersonAdded(id: string): void {
    const { scope } = this.allors;

    this.addContactPerson = false;

    const contact: Person = scope.session.get(id) as Person;

    const organisationContactRelationship = scope.session.create(
      'OrganisationContactRelationship',
    ) as OrganisationContactRelationship;
    organisationContactRelationship.Organisation = this.workTask
      .Customer as Organisation;
    organisationContactRelationship.Contact = contact;

    this.contacts.push(contact);
    this.workTask.ContactPerson = contact;
  }

  public contactMechanismAdded(
    partyContactMechanism: PartyContactMechanism,
  ): void {
    this.addContactMechanism = false;

    this.contactMechanisms.push(partyContactMechanism.ContactMechanism);
    this.workTask.Customer.AddPartyContactMechanism(partyContactMechanism);
    this.workTask.FullfillContactMechanism =
      partyContactMechanism.ContactMechanism;
  }

  public contactMechanismCancelled() {
    this.addContactMechanism = false;
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public save(): void {
    const { scope } = this.allors;

    if (this.assignees) {
      this.assignees.forEach((assignee: Person) => {
        if (this.existingAssignees.indexOf(assignee) < 0) {
          const workEffortAssignment: WorkEffortPartyAssignment = scope.session.create(
            'WorkEffortAssignment',
          ) as WorkEffortPartyAssignment;
          workEffortAssignment.Assignment = this.workTask;
          workEffortAssignment.Party = assignee;
        }
      });
    }
    scope.save().subscribe(
      (saved: Saved) => {
        this.goBack();
      },
      (error: Error) => {
        this.errorService.handle(error);
      },
    );
  }

  public goBack(): void {
    window.history.back();
  }

  private updateCustomer(party: Party) {

    const { m, pull, scope } = this.allors;

    const pulls = [
      pull.Party({
        object: party,
        fetch: {
          CurrentPartyContactMechanisms: {
            include: {
              ContactMechanism: {
                PostalAddress_PostalBoundary: {
                  Country: x,
                }
              }
            }
          }
        }
      }),
      pull.Party({
        object: party,
        fetch: {
          CurrentContacts: x,
        }
      })
    ];

    scope.load('Pull', new PullRequest({ pulls })).subscribe(
      (loaded) => {
        const partyContactMechanisms: PartyContactMechanism[] = loaded
          .collections.partyContactMechanisms as PartyContactMechanism[];
        this.contactMechanisms = partyContactMechanisms.map(
          (v: PartyContactMechanism) => v.ContactMechanism,
        );
        this.contacts = loaded.collections.currentContacts as Person[];
      },
      (error: Error) => {
        this.errorService.handle(error);
        this.goBack();
      },
    );
  }
}
