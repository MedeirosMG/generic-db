import { EventEmitter, Output, SimpleChanges } from '@angular/core';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { entityPayload } from 'src/app/interfaces/api/entityPayload';
import { tableAttributePayload } from 'src/app/interfaces/api/tableAttributePayload';
import { tablePayload } from 'src/app/interfaces/api/tablePayload';
import { Database } from 'src/app/interfaces/database/database';
import { Table } from 'src/app/interfaces/database/table';
import { attributeForm } from 'src/app/interfaces/forms/attributeForm';
import { CreateEntityComponent } from 'src/app/services/database-forms/create-entity/create-entity.component';
import { CreateTableComponent } from 'src/app/services/database-forms/create-table/create-table.component';
import { DeleteEntityComponent } from 'src/app/services/database-forms/delete-entity/delete-entity.component';
import { DeleteTableComponent } from 'src/app/services/database-forms/delete-table/delete-table.component';
import { GenerateDumpComponent } from 'src/app/services/database-forms/generate-dump/generate-dump.component';
import { DatabaseService } from 'src/app/services/database-service';
import { EntityService } from 'src/app/services/entity-service';
import { TableService } from 'src/app/services/table-service';

@Component({
  selector: 'app-generic-database',
  templateUrl: './generic-database.component.html',
  styleUrls: ['./generic-database.component.css']
})
export class GenericDatabaseComponent implements OnInit {

  constructor(
    public dialog: MatDialog,
    private databaseService: DatabaseService,
    private tableService: TableService,
    private entityService: EntityService) { }

  @Output() delete: EventEmitter<string[]> = new EventEmitter();
  @Input() database: Database;
  tables: Table[] = [];

  generateEmtpyObj(displayerColumn: string[]): any[] {
    var result = {}

    displayerColumn.forEach((item) => {
      result[item] = ""
    })

    return [result]
  }

  reloadDatabase(db: string) {

    this.databaseService.getDatabase(db).subscribe(
      (res: Database) => {
        this.database = res
        this.loadTables()
      }
    )
  }

  reloadTable(table: Table) {
    this.tableService.getTable(table.id).subscribe(
      (res: Table) => {
        this.database.tables.filter(x => x.id == res.id)[0].entities = res.entities
        this.loadTables()
      }
    )
  }

  loadTables() {
    if (!this.database)
      return;

    this.tables = []
    this.database.tables.forEach(element => {
      this.tables.push({
        name: element.name,
        displayedColumns: element.attributes.map(function (x) { return x.name }),
        entities: element.entities.length == 0 ? this.generateEmtpyObj(element.attributes.map(function (x) { return x.name })) : element.entities,
        attributes: element.attributes,
        openState: true
      } as Table);
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    this.loadTables()
  }

  ngOnInit(): void {
    this.loadTables()
  }

  parseAttributes(attributesForm: attributeForm[]): tableAttributePayload[] {
    var result: tableAttributePayload[] = []

    attributesForm.forEach((item) => {
      result.push({
        name: item.name,
        type: item.type,
        referenceAttribute: item.function === "REFERENCE",
        primaryKey: item.function === "PK"
      } as tableAttributePayload)
    })

    return result
  }

  addTable() {
    this.dialog.open(CreateTableComponent, { width: "80%" }).afterClosed().subscribe(
      data => {
        if (!data)
          return

        this.tableService.createTable({
          attributes: this.parseAttributes(data.attributes),
          tableName: data.name,
          databaseKey: this.database.id
        } as tablePayload).subscribe(
          (res: Table) => {
            this.database.tables.push(res)
            this.loadTables()
          }
        )
      }
    )
  }

  parseEntity(attributes: attributeForm[]): any {
    var result = {}
    attributes.forEach((item) => {
      result[item.name] = item.value ?? ""
    })

    return result
  }

  addEntity() {
    this.dialog.open(CreateEntityComponent, { width: "80%", data: this.database.tables }).afterClosed().subscribe(
      data => {
        if (!data)
          return

        this.entityService.createEntity({
          databaseKey: this.database.id,
          tableKey: data.tableId,
          entity: this.parseEntity(data.attributes)
        } as entityPayload).subscribe(
          (res: Table) => {
            this.reloadTable(res)
          }
        )
      }
    )
  }

  generateDump() {
    this.dialog.open(GenerateDumpComponent, { width: "80%", maxHeight: "80%", data: this.database.id })
  }

  deleteDb() {
    this.databaseService.deleteDatabase(this.database.id).subscribe(
      (res: string[]) => {
        this.delete.emit(res)
      }
    )
  }

  deleteTable() {
    this.dialog.open(DeleteTableComponent, { width: "20%", data: this.database.tables }).afterClosed().subscribe(
      data => {
        if (!data)
          return

        this.tableService.deleteTable(this.database.id, data).subscribe(
          (res: Database) => {
            this.reloadDatabase(res.id)
          }
        )
      }
    )
  }

  deleteEntity() {
    this.dialog.open(DeleteEntityComponent, { width: "80%", data: this.database.tables }).afterClosed().subscribe(
      data => {
        if (!data)
          return

        this.entityService.deleteEntity(this.database.id, data.tableId, data.entitiesId).subscribe(
          (res: Table) => {
            this.reloadTable(res)
          }
        )
      }
    )
  }
}
