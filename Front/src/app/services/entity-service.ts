import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { defaultApiResponse } from '../interfaces/api/defaultApiResponse';
import { entityPayload } from '../interfaces/api/entityPayload';
import { Table } from '../interfaces/database/table';
import { GlobalDialogComponent } from '../shared/global-dialog/global-dialog.component';
import { ApiService } from './api-service';

@Injectable({
    providedIn: 'root'
})
export class EntityService {
    constructor(
        private apiService: ApiService,
        public dialog: MatDialog) { }

    createEntity(entity: entityPayload): Observable<Table> {
        return new Observable<Table>(obs => {
            this.apiService.post(entity, "/Database/Create/entity").subscribe(
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

    deleteEntity(dbId: string, tableId: string, entitiesId: number[]): Observable<Table>{
        return new Observable<Table>(obs => {
            this.apiService.post(entitiesId, "/Database/Delete/entity/" + dbId + "/" + tableId).subscribe(
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