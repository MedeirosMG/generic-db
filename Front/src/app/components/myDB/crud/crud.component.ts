import { CreateDatabaseComponent } from 'src/app/services/database-forms/create-database/create-database.component';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Database } from 'src/app/interfaces/database/database';
import { DatabaseService } from 'src/app/services/database-service';
import * as dataSampleDb from '../../../..//assets/MyDatabase.json'
import { TableService } from 'src/app/services/table-service';
import { EntityService } from 'src/app/services/entity-service';
import { NgxSpinnerService } from 'ngx-spinner';
import { GlobalDialogComponent } from 'src/app/shared/global-dialog/global-dialog.component';

@Component({
  selector: 'app-crud',
  templateUrl: './crud.component.html',
  styleUrls: ['./crud.component.css']
})
export class CRUDComponent implements OnInit {

  constructor(
    private dialog: MatDialog,
    private databaseService: DatabaseService) { }

  databases: string[] = []
  databasesObject: Database[] = []

  createDatabase() {
    this.dialog.open(CreateDatabaseComponent).afterClosed().subscribe(
      data => {
        if (!data)
          return

        this.databaseService.createDatabase(data).subscribe(
          (res: Database) => {
            this.databasesObject.push(res)
          }
        )
      }
    )
  }

  getDatabases() {
    this.databasesObject = []

    this.databases.forEach(element => {
      this.databaseService.getDatabase(element).subscribe(
        (res: Database) => {
          this.databasesObject.push(res)
        }
      )
    })
  }

  reloadDBs(listDbs: string[]) {
    this.databases = listDbs
    this.getDatabases()
  }

  sampleDb() {
    var databaseSample: Database = dataSampleDb.default as Database
    this.databaseService.createDatabaseByDump(databaseSample).subscribe(
      (res: Database) => {
        this.initDatabases()
      }
    )
  }

  initDatabases() {
    this.databaseService.getListDatabases().subscribe(
      (res: string[]) => {
        this.databases = res
        if (res.length == 0) {
          this.sampleDb()
          this.dialog.open(GlobalDialogComponent, {
            data: { title: "Information", message: "Empty data! Sample 'My Database' was created" }
          })
        }

        this.getDatabases();
      }
    )
  }

  ngOnInit() {
    this.initDatabases()
  }
}
