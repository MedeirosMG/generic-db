import { Injectable } from '@angular/core';

import { retry, catchError } from 'rxjs/operators';
import { Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiService {

    // REST API
    endpoint = environment.endpointAPI;
    constructor(private httpClient: HttpClient) { }

    httpHeader = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        })
    }

    get(queryString: string, route: string): Observable<any> {
        return this.httpClient.get<any>(this.endpoint + route + "?" + queryString)
            .pipe(
                retry(1),
                catchError((error) => this.processError(error))
            )
    }

    post(data: any, route: string): Observable<any> {
        return this.httpClient.post<any>(this.endpoint + route, JSON.stringify(data), this.httpHeader)
            .pipe(
                retry(1),
                catchError((error) => this.processError(error))
            )
    }

    delete(queryString: string, route: string): Observable<any> {
        return this.httpClient.delete<any>(this.endpoint + route + "?" + queryString)
            .pipe(
                retry(1),
                catchError((error) => this.processError(error))
            )
    }

    processError(err) {
        let message = '';

        if (err.error && err.error.content) {
            message = err.error.message;
        } else if (err.error instanceof ErrorEvent) {
            message = err.error.message;
        } else {
            message = `Error Code: ${err.status}\nMessage: ${err.message}`;
        }

        return throwError(message)
    }

}