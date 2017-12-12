import {Component, Inject} from "@angular/core";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material";

@Component({
  selector: "newgood-dialog",
  template: `
  <h4 mat-dialog-title>Choose Product</h4>
  <p>Select 'Serialised' if the prodcut is unique and has a serialnumber.</p>
  <p>Select 'NonSerialised' if the product is a group of items and you want to keep stock.</p>

  <div mat-dialog-content>
  <mat-form-field>
    <mat-select [value]="data.chosenGood" [(ngModel)]="data.chosenGood">
      <mat-option value="Serialised">Serialised</mat-option>
      <mat-option value="NonSerialised">NonSerialised</mat-option>
    </mat-select>
    </mat-form-field>
  </div>

  <div mat-dialog-actions align="end">
    <button mat-button (click)="onCancelClick()">Cancel</button>
    <button mat-button [mat-dialog-close]="data.chosenGood" >OK</button>
  </div>
  `,
})

export class NewGoodDialogComponent {
  constructor(public dialogRef: MatDialogRef<NewGoodDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {}

  public onCancelClick(): void {
    this.dialogRef.close();
  }
}
