import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy , OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { TdMediaService } from "@covalent/core";
import { Subscription } from "rxjs/Subscription";

import { ErrorService, Loaded, Saved, Scope, WorkspaceService } from "../../../../../../angular";
import { Country, Enumeration, Party, PartyContactMechanism, PostalAddress, PostalBoundary } from "../../../../../../domain";
import { Fetch, PullRequest, Query, TreeNode } from "../../../../../../framework";
import { MetaDomain } from "../../../../../../meta";

@Component({
  templateUrl: "./party-contactmechanism-postaladdress.html",
})
export class PartyContactMechanismPostalAddressAddComponent implements OnInit, OnDestroy {

  public title: string = "Postal Address";
  public subTitle: string = "add postal address";

  public m: MetaDomain;

  public party: Party;
  public partyContactMechanism: PartyContactMechanism;
  public contactMechanism: PostalAddress;
  public postalBoundary: PostalBoundary;
  public contactMechanismPurposes: Enumeration[];
  public countries: Country[];

  private subscription: Subscription;
  private scope: Scope;

  constructor(
    private workspaceService: WorkspaceService,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    public media: TdMediaService, private changeDetectorRef: ChangeDetectorRef) {

    this.scope = this.workspaceService.createScope()
    this.m = this.workspaceService.metaPopulation.metaDomain;
  }

  ngOnInit(): void {
    this.subscription = this.route.url
      .switchMap((url: any) => {

        const id: string = this.route.snapshot.paramMap.get("id");
        const m: MetaDomain = this.m;

        const fetch: Fetch[] = [
          new Fetch({
            name: "party",
            id,
            include: [
              new TreeNode({
                roleType: m.Party.PartyContactMechanisms,
                nodes: [
                  new TreeNode({ roleType: m.PartyContactMechanism.ContactPurposes }),
                  new TreeNode({
                    roleType: m.PartyContactMechanism.ContactMechanism,
                    nodes: [
                      new TreeNode({
                        roleType: m.PostalAddress.PostalBoundary,
                        nodes: [
                          new TreeNode({ roleType: m.PostalBoundary.Country }),
                        ],
                      }),
                    ],
                  }),
                ],
              }),
            ],
          }),
        ];

        const query: Query[] = [
          new Query(
            {
              name: "contactMechanismPurposes",
              objectType: this.m.ContactMechanismPurpose,
            }),
          new Query(
            {
              name: "countries",
              objectType: this.m.Country,
            }),
        ];

        return this.scope
          .load("Pull", new PullRequest({ fetch, query }));
      })
      .subscribe((loaded: Loaded) => {

        this.party = loaded.objects.party as Party;

        if (!this.contactMechanism) {
          this.contactMechanism = this.scope.session.create("PostalAddress") as PostalAddress;
          this.postalBoundary = this.scope.session.create("PostalBoundary") as PostalBoundary;
          this.contactMechanism.PostalBoundary = this.postalBoundary;
        }

        this.partyContactMechanism = this.scope.session.create("PartyContactMechanism") as PartyContactMechanism;
        this.partyContactMechanism.ContactMechanism = this.contactMechanism;
        this.partyContactMechanism.UseAsDefault = true;

        this.party.AddPartyContactMechanism(this.partyContactMechanism);

        this.contactMechanismPurposes = loaded.collections.contactMechanismPurposes as Enumeration[];
        this.countries = loaded.collections.countries as Country[];
      },
      (error: any) => {
        this.errorService.message(error);
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

    this.scope
      .save()
      .subscribe((saved: Saved) => {
        this.goBack();
      },
      (error: Error) => {
        this.errorService.dialog(error);
      });
  }

  public goBack(): void {
    window.history.back();
  }
}
