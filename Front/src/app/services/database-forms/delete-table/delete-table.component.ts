import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Table } from 'src/app/interfaces/database/table';

@Component({
  selector: 'app-delete-table',
  templateUrl: './delete-table.component.html',
  styleUrls: ['./delete-table.component.css']
})
export class DeleteTableComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteTableComponent>,
    @Inject(MAT_DIALOG_DATA) public tables: Table[]) { }

  tableId: string
  ngOnInit(): void {
  }

  selectTable(table) {
    this.tableId = table.value
  }

  close() {
    this.dialogRef.close();
  }

  delete() {
    this.dialogRef.close(this.tableId)
  }
}
