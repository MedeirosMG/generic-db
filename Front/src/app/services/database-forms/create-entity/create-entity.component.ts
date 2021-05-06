import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Table } from 'src/app/interfaces/database/table';
import { tableAttribute } from 'src/app/interfaces/database/tableAttributes';
import { attributeForm } from 'src/app/interfaces/forms/attributeForm';
import { entityForm } from 'src/app/interfaces/forms/entityForm';


@Component({
  selector: 'app-create-entity',
  templateUrl: './create-entity.component.html',
  styleUrls: ['./create-entity.component.css']
})
export class CreateEntityComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<CreateEntityComponent>,
    @Inject(MAT_DIALOG_DATA) public tables: Table[]) { }

  tableSelectedId: string
  entityForm: entityForm

  ngOnInit(): void {
    console.log(this.tables)
  }

  getTableById(idTable: string): Table[] {
    return this.tables.filter(x => x.id == idTable)
  }

  parseAttributes(tableAttributes: tableAttribute[]): attributeForm[] {
    var result: attributeForm[] = []
    tableAttributes.forEach((item) => {
      result.push({
        name: item.name,
        type: item.type,
        function: item.primaryKey ? "PK" : (item.referenceAttribute ? "REFERENCE" : "OTHER")
      } as attributeForm)
    })

    return result
  }

  generateNewId(idTable: string) {
    var table = this.tables.filter(x => x.id == idTable)[0]
    if (!table.entities || table.entities.length == 0)
      return 1

    var maxValue = table.entities[table.entities.length - 1][table.attributes.filter(x => x.primaryKey)[0].name]
    return maxValue + 1
  }

  selectTable(event) {
    var table = this.getTableById(event.value)
    this.tableSelectedId = event.value
    if (!table)
      return

    this.entityForm = { tableId: event.value, attributes: [] } as entityForm

    this.entityForm.attributes = this.parseAttributes(table[0].attributes)
    this.entityForm.attributes.filter(x => x.function == "PK")[0].value = this.generateNewId(event.value);
  }

  getPk(): attributeForm[] {
    if (!this.entityForm)
      return []

    return this.entityForm.attributes.filter(x => x.function == "PK")
  }

  getReferences(): attributeForm[] {
    if (!this.entityForm)
      return []

    return this.entityForm.attributes.filter(x => x.function == "REFERENCE")
  }

  getOthersAttributes(boolType): attributeForm[] {
    if (!this.entityForm)
      return []

    if (boolType)
      return this.entityForm.attributes.filter(x => x.function == "OTHER" && x.type === "boolean")

    return this.entityForm.attributes.filter(x => x.function == "OTHER" && x.type !== "boolean")
  }

  addEntity() {
    this.dialogRef.close(this.entityForm)
  }

  close() {
    this.dialogRef.close();
  }

  getTablesForReferences() {
    return this.tables.filter(x => x.id != this.tableSelectedId)
  }

  selectTableReference(event, item) {
    if (!item)
      return

    item.value = event.value + ".."
  }

  selectAttributeReference(event, item) {
    if (!item || !item.value)
      return

    var splitedValue = item.value.split(".")
    splitedValue[1] = "" // Entity Id
    splitedValue[2] = event.value // Attribute Id

    item.value = splitedValue.join(".")
  }

  entityReference(event, item) {
    if (!item || !item.value)
      return

    var splitedValue = item.value.split(".")
    splitedValue[1] = event // EntityId

    item.value = splitedValue.join(".")
  }

  getAttributeReference(item): tableAttribute[] {
    if (!item.value)
      return []

    var table = this.tables.filter(x => x.id == item.value.split(".")[0])[0] as Table
    var PK = table.attributes.filter(x => x.primaryKey)[0].name
    if (!table)
      return []

    return table.attributes.filter(x => x.name != PK)
  }

  checkReferencesForm(item) {
    if (!item || !item.value)
      return false

    var splitedValue = item.value.split(".")
    return splitedValue[0] && splitedValue[2]
  }
}
