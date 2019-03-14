import { Component, Self, OnInit, HostBinding } from '@angular/core';
import { NavigationService, Action, PanelService, RefreshService, ErrorService, MetaService } from '../../../../../../angular';
import { WorkEffortInventoryAssignment, NonSerialisedInventoryItem, SerialisedInventoryItem } from '../../../../../../domain';
import { Meta } from '../../../../../../meta';
import { DeleteService, TableRow, EditService, Table, OverviewService, CreateData } from '../../../../..';
import * as moment from 'moment';

interface Row extends TableRow {
  object: WorkEffortInventoryAssignment;
  part: string;
  facility: string;
  state: string;
  quantity: number;
  uom: string;
}

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'workeffortinventoryassignment-overview-panel',
  templateUrl: './workeffortinventoryassignment-overview-panel.component.html',
  providers: [PanelService]
})
export class WorkEffortInventoryAssignmentOverviewPanelComponent implements OnInit {

  @HostBinding('class.expanded-panel') get expandedPanelClass() {
    return this.panel.isExpanded;
  }

  m: Meta;

  objects: WorkEffortInventoryAssignment[] = [];

  edit: Action;
  table: Table<TableRow>;

  get createData(): CreateData {
    return {
      associationId: this.panel.manager.id,
      associationObjectType: this.panel.manager.objectType,
    };
  }

  constructor(
    @Self() public panel: PanelService,
    public metaService: MetaService,
    public refreshService: RefreshService,
    public navigation: NavigationService,
    public errorService: ErrorService,
    public deleteService: DeleteService,
    public editService: EditService,
  ) {

    this.m = this.metaService.m;
  }

  ngOnInit() {

    this.edit = this.editService.edit();

    this.panel.name = 'workeffortinventoryassignment';
    this.panel.title = 'Work Effort Inventory Assignment';
    this.panel.icon = 'work';
    this.panel.expandable = true;

    const sort = true;
    this.table = new Table({
      selection: true,
      columns: [
        { name: 'part' },
        { name: 'facility' },
        { name: 'state' },
        { name: 'quantity' },
        { name: 'uom' },
      ],
      actions: [
        this.edit,
      ],
      defaultAction: this.edit,
      autoSort: true,
      autoFilter: true,
    });

    const pullName = `${this.panel.name}_${this.m.WorkEffortInventoryAssignment.name}`;

    this.panel.onPull = (pulls) => {
      const { pull, x } = this.metaService;

      const id = this.panel.manager.id;

      pulls.push(
        pull.WorkEffort({
          name: pullName,
          object: id,
          fetch: {
            WorkEffortInventoryAssignmentsWhereAssignment: {
              include: {
                InventoryItem: {
                  Part: x,
                  Facility: x,
                  UnitOfMeasure: x,
                  NonSerialisedInventoryItem_NonSerialisedInventoryItemState: x,
                  SerialisedInventoryItem_SerialisedInventoryItemState: x
                }
              }
            }
          }
        }),
      );
    };

    this.panel.onPulled = (loaded) => {
      this.objects = loaded.collections[pullName] as WorkEffortInventoryAssignment[];

      if (this.objects) {
        this.table.total = this.objects.length;
        this.table.data = this.objects.map((v) => {
          return {
            object: v,
            part: v.InventoryItem.Part.Name,
            facility: v.InventoryItem.Facility.Name,
            state: (v.InventoryItem as NonSerialisedInventoryItem).NonSerialisedInventoryItemState.Name || (v.InventoryItem as SerialisedInventoryItem).SerialisedInventoryItemState.Name,
            quantity: v.Quantity,
            uom: v.InventoryItem.UnitOfMeasure.Name,
          } as Row;
        });
      }
    };
  }
}