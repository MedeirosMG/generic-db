import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { observable, Observable } from 'rxjs';
import { defaultApiResponse } from '../interfaces/api/defaultApiResponse';
import { entityPayload } from '../interfaces/api/entityPayload';
import { tableAttributePayload } from '../interfaces/api/tableAttributePayload';
import { tablePayload } from '../interfaces/api/tablePayload';
import { Database } from '../interfaces/database/database';
import { Table } from '../interfaces/database/table';
import { tableAttribute } from '../interfaces/database/tableAttributes';
import { GlobalDialogComponent } from '../shared/global-dialog/global-dialog.component';
import { ApiService } from './api-service';
import { EntityService } from './entity-service';
import { TableService } from './table-service';
import { forkJoin } from 'rxjs';
import { CreateDatabaseComponent } from './database-forms/create-database/create-database.component';
import { Subscriber } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DatabaseService {
    constructor(
        private apiService: ApiService,
        private dialog: MatDialog,
        private tableService: TableService,
        private entityService: EntityService) { }

    private recursiveAddTables(database: Database, idDatabase: string, obsToResolt: Subscriber<Database>, index: number) {
        if (database.tables.length == index){
            obsToResolt.next(database)
            obsToResolt.complete()
            return
        }

        this.tableService.createTableByDump(idDatabase, database.tables[index]).subscribe(
            (resultTable: Table) => {
                var oldId = database.tables[index].id
                var newId = resultTable.id

                for (var i = 0; i < database.tables.length; i++) {
                    var jsonObject = JSON.stringify(database.tables[i]).split(oldId).join(newId)
                    database.tables[i] = JSON.parse(jsonObject)
                }

                var tasks$ = [];
                database.tables[index].entities.forEach((entity: any) => {
                    tasks$.push(this.entityService.createEntity({
                        databaseKey: idDatabase,
                        tableKey: resultTable.id,
                        entity: entity
                    } as entityPayload))
                })

                forkJoin(tasks$).subscribe(
                    results => {
                        this.recursiveAddTables(database, idDatabase, obsToResolt, index + 1)
                    });
            });
    }

    createDatabaseByDump(database: Database): Observable<Database> {
        return new Observable<Database>(obs => {
            this.createDatabase(database.name).subscribe(
                (resDatabase: Database) => {
                    // Success, go for tables
                    this.recursiveAddTables(database, resDatabase.id, obs, 0)
                },
                error => {
                    this.dialog.open(GlobalDialogComponent, {
                        data: { title: "Error", message: error }
                    })

                    obs.error(error)
                    obs.complete()
                })
        })
    }

    createDatabase(dbName: string): Observable<Database> {
        return new Observable<Database>(obs => {
            this.apiService.get("dbName=" + dbName, "/Database/Create/db").subscribe(
                (res: defaultApiResponse<Database>) => {
                    obs.next(res.content)
                    obs.complete()
                },
                error => {
                    this.dialog.open(GlobalDialogComponent, {
                        data: { title: "Error", message: error }
                    })

                    obs.error(error)
                    obs.complete()
                })
        })
    }

    deleteDatabase(dbId: string): Observable<string[]> {
        return new Observable<string[]>((obs) => {
            this.apiService.delete("dbId=" + dbId, "/Database/Delete/db").subscribe(
                (res: defaultApiResponse<string[]>) => {
                    obs.next(res.content)
                    obs.complete()
                },
                error => {
                    this.dialog.open(GlobalDialogComponent, {
                        data: { title: "Error", message: error }
                    })

                    obs.error(error)
                    obs.complete()
                })
        })
    }

    getDump(dbId: string): Observable<Database> {
        return new Observable<Database>(obs => {
            this.apiService.get("dbId=" + dbId, "/Database/Query/dump")
                .subscribe(
                    (res: defaultApiResponse<Database>) => {
                        obs.next(res.content)
                        obs.complete()
                    },
                    error => {
                        this.dialog.open(GlobalDialogComponent, {
                            data: { title: "Error", message: error }
                        })

                        obs.error(error)
                        obs.complete()
                    })
        })
    }

    /* Get Functions */
    getListDatabases(): Observable<string[]> {
        return new Observable<string[]>(obs => {
            this.apiService.get("", "/Database/Query/listDBs")
                .subscribe(
                    (res: defaultApiResponse<string[]>) => {
                        obs.next(res.content)
                        obs.complete()
                    },
                    error => {
                        this.dialog.open(GlobalDialogComponent, {
                            data: { title: "Error", message: error }
                        })

                        obs.error(error)
                        obs.complete()
                    }
                )
        })
    }

    getDatabase(dbId: string): Observable<Database> {
        return new Observable<Database>(obs => {
            this.apiService.get("dbId=" + dbId, "/Database/Query/fullDb").subscribe(
                (res: defaultApiResponse<Database>) => {
                    obs.next(res.content)
                },
                error => {
                    this.dialog.open(GlobalDialogComponent, {
                        data: { title: "Error", message: error }
                    })

                    obs.error(error)
                    obs.complete()
                }
            )
        });
    }
}