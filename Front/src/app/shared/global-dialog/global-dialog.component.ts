import { Component, Output, EventEmitter, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-global-dialog',
  templateUrl: './global-dialog.component.html',
  styleUrls: ['./global-dialog.component.css']
})
export class GlobalDialogComponent implements OnInit {

  localStorageVar: string = "globalDialogMessages"
  listError: string[]
  constructor(
    private matDialog: MatDialog,
    private dialogRef: MatDialogRef<GlobalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    if (!localStorage.getItem(this.localStorageVar))
      this.listError = []
    else
      this.listError = JSON.parse(localStorage.getItem(this.localStorageVar))

    if (this.matDialog.openDialogs.length < 2)
      this.listError = []

    if (this.data.message) {
      this.listError.push(this.data.message)
    } else if (this.data.listError) {
      this.listError.join(this.data.listError)
    } else {
      this.dialogRef.close()
      return
    }

    localStorage.setItem(this.localStorageVar, JSON.stringify(this.listError))
  }

  close() {
    this.matDialog.closeAll()
    localStorage.setItem(this.localStorageVar, JSON.stringify([]))
  }

}
