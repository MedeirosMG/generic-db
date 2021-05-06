import { Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Table } from 'src/app/interfaces/database/table';
import { attributeForm } from 'src/app/interfaces/forms/attributeForm';
import { deleteEntityForm } from 'src/app/interfaces/forms/deleteEntityForm';

@Component({
  selector: 'app-delete-entity',
  templateUrl: './delete-entity.component.html',
  styleUrls: ['./delete-entity.component.css']
})
export class DeleteEntityComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteEntityComponent>,
    @Inject(MAT_DIALOG_DATA) public tables: Table[]) { }

  deleteEntityForm: deleteEntityForm
  tableSelected: Table
  @ViewChild("entitiesSelected") entities

  ngOnInit(): void {
    this.deleteEntityForm = { entitiesId: [] } as deleteEntityForm
  }

  selectTable(table) {
    this.tableSelected = this.tables.filter(x => x.id == table.value)[0]
    this.deleteEntityForm.tableId = this.tableSelected.id
  }

  close() {
    this.dialogRef.close();
  }

  // Get all object propertie
  getProperties(object): string[] {
    if (!object)
      return []

    return Object.getOwnPropertyNames(object)
  }

  parseModelText(model) {
    var result = ""
    this.getProperties(model).forEach((item, index) => {
      result += (item + ": " + model[item])

      if (index !== this.getProperties(model).length - 1)
        result += " | "
    })

    return result
  }

  delete() {
    var PK = this.tableSelected.attributes.filter(x => x.primaryKey)[0].name
    this.entities.selectedOptions.selected.forEach(item => { this.deleteEntityForm.entitiesId.push(item.value[PK]) })

    this.dialogRef.close(this.deleteEntityForm)
  }
}
