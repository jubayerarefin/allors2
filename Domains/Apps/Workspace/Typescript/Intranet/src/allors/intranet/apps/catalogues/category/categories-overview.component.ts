import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MatSnackBar } from "@angular/material";
import { Title } from "@angular/platform-browser";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, Subscription } from "rxjs/Rx";

import { TdDialogService, TdMediaService } from "@covalent/core";

import { AllorsService, ErrorService, Loaded,  Scope } from "@allors";
import { And, Like, Page, Predicate, PullRequest, Query, TreeNode } from "@allors";
import { ProductCategory } from "@allors";
import { MetaDomain } from "@allors";

interface SearchData {
  name: string;
}

@Component({
  templateUrl: "./categories-overview.component.html",
})
export class CategoriesOverviewComponent implements AfterViewInit, OnDestroy {

  public title: string = "Categories";
  public total: number;
  public searchForm: FormGroup;
  public data: ProductCategory[];
  public filtered: ProductCategory[];
  private refresh$: BehaviorSubject<Date>;
  private page$: BehaviorSubject<number>;

  private subscription: Subscription;
  private scope: Scope;

  constructor(
    private allors: AllorsService,
    private errorService: ErrorService,
    private formBuilder: FormBuilder,
    private titleService: Title,
    private snackBar: MatSnackBar,
    private router: Router,
    private dialogService: TdDialogService,
    public media: TdMediaService, private changeDetectorRef: ChangeDetectorRef) {

    this.titleService.setTitle("Categories");

    this.scope = new Scope(allors.database, allors.workspace);
    this.refresh$ = new BehaviorSubject<Date>(undefined);

    this.searchForm = this.formBuilder.group({
      name: [""],
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

        if (data.name) {
          const like: string = data.name.replace("*", "%") + "%";
          predicates.push(new Like({ roleType: m.ProductCategory.Name, value: like }));
        }

        const query: Query[] = [new Query(
          {
            include: [
              new TreeNode({ roleType: m.ProductCategory.CategoryImage }),
              new TreeNode({ roleType: m.ProductCategory.LocalisedNames }),
              new TreeNode({ roleType: m.ProductCategory.LocalisedDescriptions }),
            ],
            name: "categories",
            objectType: m.ProductCategory,
            page: new Page({ skip: 0, take }),
            predicate,
          })];

        return this.scope.load("Pull", new PullRequest({ query }));

      })
      .subscribe((loaded: Loaded) => {
        this.data = loaded.collections.categories as ProductCategory[];
        this.total = loaded.values.categories_total;
      },
      (error: any) => {
        this.errorService.message(error);
        this.goBack();
      });
  }

  public more(): void {
    this.page$.next(this.data.length + 50);
  }

  public goBack(): void {
    window.history.back();
  }

  public ngAfterViewInit(): void {
    this.media.broadcast();
    this.changeDetectorRef.detectChanges();
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public delete(category: ProductCategory): void {
    this.dialogService
      .openConfirm({ message: "Are you sure you want to delete this category?" })
      .afterClosed().subscribe((confirm: boolean) => {
        if (confirm) {
          // TODO: Logical, physical or workflow delete
        }
      });
  }

  public onView(category: ProductCategory): void {
    this.router.navigate(["/category/" + category.id]);
  }
}
