import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CovalentLayoutModule } from "@covalent/core";

import { MatAutocompleteModule, MatCardModule, MatIconModule, MatListModule, MatToolbarModule } from "@angular/material";

import { ChipsModule } from "@baseCovalent/index";
import { StaticModule } from "@baseMaterial/index";

import { RelationsComponent } from "./relations.component";
export { RelationsComponent } from "./relations.component";

@NgModule({
  declarations: [
    RelationsComponent,
  ],
  exports: [
    RelationsComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    MatAutocompleteModule,
    MatCardModule,
    MatIconModule,
    MatListModule,
    MatToolbarModule,
    CovalentLayoutModule,
    ChipsModule,
    StaticModule,
  ],
})
export class RelationsModule {}
