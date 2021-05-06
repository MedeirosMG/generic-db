import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { attributeForm } from 'src/app/interfaces/forms/attributeForm';
import { tableForm } from 'src/app/interfaces/forms/tableForm';

interface Food {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-create-table',
  templateUrl: './create-table.component.html',
  styleUrls: ['./create-table.component.css']
})
export class CreateTableComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<CreateTableComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,) { }

  attributes: attributeForm[] = [];
  selectedList: attributeForm[]
  tableForm: tableForm

  formError: string = ""
  typeAttribute: string
  nameAttribute: string
  functionAttribute: string
  tableName: string = "Table Name"

  displayedColumns: string[] = ['name', 'type', 'function'];

  attributesType: Food[] = [
    { value: 'string', viewValue: 'String' },
    { value: 'boolean', viewValue: 'Boolean' },
    { value: 'number', viewValue: 'Scalar' }
  ];

  ngOnInit(): void {
    this.typeAttribute = "number"
    this.functionAttribute = "PK"
  }

  selectList(selectedList: attributeForm) {
    this.selectedList = [selectedList]
  }

  deleteCurrente() {
    this.attributes = this.attributes.filter(item => item.name !== this.selectedList[0].name)
    this.selectedList = []
  }

  pkDefined() {
    return this.attributes.filter(item => item.function === "PK").length > 0
  }

  resetForm() {
    this.functionAttribute = ""
    this.typeAttribute = ""
    this.nameAttribute = ""
  }

  addAttribute() {

    if (!this.functionAttribute || !this.typeAttribute || !this.nameAttribute) {
      this.formError = "Invalid attributes"
      return
    }

    if (this.attributes.filter(item => item.name === this.nameAttribute).length != 0) {
      this.formError = "Id '" + this.nameAttribute + "' attributes already in use"
      return
    }

    this.formError = ""
    this.attributes.push({
      name: this.nameAttribute,
      type: this.typeAttribute,
      function: this.functionAttribute
    } as attributeForm)

    let sortOrder = ['PK', 'REFERENCE', 'OTHER'];
    this.attributes.sort((a, b) => {
      return sortOrder.indexOf(a.function) - sortOrder.indexOf(b.function);
    });

    this.resetForm()
  }

  typeSelected(event) {
    if (event == "REFERENCE")
      this.typeAttribute = "string"
    else if (event == "PK")
      this.typeAttribute = "number"
  }

  createTable() {
    this.dialogRef.close({ attributes: this.attributes, name: this.tableName } as tableForm)
  }

  close() {
    this.dialogRef.close();
  }

}
