import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GlobalDialogComponent } from 'src/app/shared/global-dialog/global-dialog.component';

@Component({
  selector: 'app-create-database',
  templateUrl: './create-database.component.html',
  styleUrls: ['./create-database.component.css']
})
export class CreateDatabaseComponent implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<CreateDatabaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialog: MatDialog) { }

  dbName: string;
  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close();
  }

  create() {

    if (!this.dbName) {
      this.dialog.open(GlobalDialogComponent, {
        width: "40%",
        data: { title: "Error", message: "DBName invalid" }
      })

      this.dialogRef.close()
    }

    this.dialogRef.close(this.dbName)
  }
}
