import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatCardModule} from '@angular/material/card';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatTableModule} from '@angular/material/table';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatIconModule} from '@angular/material/icon'
import { CRUDComponent } from './components/myDB/crud/crud.component';
import { GenericTableComponent } from './components/myDB/crud/generic-table/generic-table.component';
import {MatButtonModule} from '@angular/material/button';
import { GenericDatabaseComponent } from './components/myDB/crud/generic-database/generic-database.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApiService } from './services/api-service';
import {MatDialogModule} from '@angular/material/dialog';
import { GlobalDialogComponent } from './shared/global-dialog/global-dialog.component';
import {MatToolbarModule} from '@angular/material/toolbar';
import { CreateDatabaseComponent } from './services/database-forms/create-database/create-database.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {MatListModule} from '@angular/material/list';
import { CreateTableComponent } from './services/database-forms/create-table/create-table.component';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatRadioModule} from '@angular/material/radio';
import { CreateEntityComponent } from './services/database-forms/create-entity/create-entity.component';
import {MatSelectModule} from '@angular/material/select';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { GenerateDumpComponent } from './services/database-forms/generate-dump/generate-dump.component';
import {MatTabsModule} from '@angular/material/tabs';
import { DeleteTableComponent } from './services/database-forms/delete-table/delete-table.component';
import { DeleteEntityComponent } from './services/database-forms/delete-entity/delete-entity.component';
import { DatabaseService } from './services/database-service';
import { TableService } from './services/table-service';
import { EntityService } from './services/entity-service';
import { LoaderScreenComponent } from './shared/loader-screen/loader-screen.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { NgxSpinnerModule } from "ngx-spinner";
import { LoaderService } from './services/loader.service';
import { LoaderInterceptor } from './services/loader-interceptor.service';

@NgModule({
  declarations: [
    CRUDComponent,
    GenericTableComponent,
    GenericDatabaseComponent,
    GlobalDialogComponent,
    CreateDatabaseComponent,
    CreateTableComponent,
    CreateEntityComponent,
    GenerateDumpComponent,
    DeleteTableComponent,
    DeleteEntityComponent,
    LoaderScreenComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatCardModule,
    MatGridListModule,
    MatTableModule,
    MatExpansionModule,
    MatIconModule,
    MatButtonModule,
    HttpClientModule,
    MatDialogModule,
    MatToolbarModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatListModule,
    MatCheckboxModule,
    MatRadioModule,
    MatSelectModule,
    NgxJsonViewerModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    NgxSpinnerModule
  ],
  providers: [
    ApiService,
    DatabaseService,
    TableService,
    EntityService,
    LoaderService,
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true }
  ],
  bootstrap: [CRUDComponent]
})
export class AppModule { }
