import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin, observable, Observable } from 'rxjs';
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

@Injectable({
    providedIn: 'root'
})
export class TableService {
    constructor(
        private apiService: ApiService,
        public dialog: MatDialog,
        private entityService: EntityService) { }

    private parseAttributes(attributes: tableAttribute[]): tableAttributePayload[] {
        var result: tableAttributePayload[] = []
        attributes.forEach((attribute: tableAttribute) => {
            result.push({
                name: attribute.name,
                type: attribute.type,
                referenceAttribute: attribute.referenceAttribute,
                primaryKey: attribute.primaryKey
            } as tableAttributePayload)
        })

        return result
    }

    createTableByDump(databaseId: string, table: Table): Observable<Table> {
        return new Observable<Table>((obs) => {
            this.createTable({
                databaseKey: databaseId,
                tableName: table.name,
                attributes: this.parseAttributes(table.attributes)
            } as tablePayload).subscribe(
                (resTable: Table) => {
                    obs.next(resTable)
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

    createTable(table: tablePayload): Observable<Table> {
        return new Observable<Table>(obs => {
            this.apiService.post(table, "/Database/Create/table").subscribe(
                (res: defaultApiResponse<Table>) => {
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

    deleteTable(dbId: string, tableId: string): Observable<Database> {
        return new Observable<Database>(obs => {
            this.apiService.delete(
                "dbId=" + dbId +
                "&tableId=" + tableId, "/Database/Delete/table").subscribe(
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

    getTable(tableName: string): Observable<Table> {
        return new Observable<Table>(obs => {
            this.apiService.get("tableId=" + tableName, "/Database/Query/fullTable")
                .subscribe(
                    (res: defaultApiResponse<Table>) => {
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
}