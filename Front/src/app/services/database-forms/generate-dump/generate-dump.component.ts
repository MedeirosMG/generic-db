import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Database } from 'src/app/interfaces/database/database';
import { DatabaseService } from '../../database-service';

@Component({
  selector: 'app-generate-dump',
  templateUrl: './generate-dump.component.html',
  styleUrls: ['./generate-dump.component.css']
})
export class GenerateDumpComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<GenerateDumpComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialog: MatDialog,
    private databaseService: DatabaseService) { }

  databaseId: string
  typeGenerate: string = "dump"
  database: Database = {} as Database

  setTypeGenerator(type) {
    this.typeGenerate = type.tab.textLabel
    this.getDatabase()
  }

  getDatabase() {
    if (this.typeGenerate.toLowerCase() == "dump") {
      this.databaseService.getDump(this.databaseId).subscribe(
        (res: Database) => {
          this.database = res
        }, error => this.dialogRef.close()
      )
    } else {
      this.databaseService.getDatabase(this.databaseId).subscribe(
        (res: Database) => {
          this.database = res
        }, error => this.dialogRef.close()
      )
    }
  }

  ngOnInit(): void {
    this.databaseId = this.data
    this.getDatabase()
  }

  generateDownload() {
    var sJson = JSON.stringify(this.database);
    var element = document.createElement('a');
    element.setAttribute('href', "data:text/json;charset=UTF-8," + encodeURIComponent(sJson));
    element.setAttribute('download', this.database.name + ".json");
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click(); // simulate click
    document.body.removeChild(element);
  }

  close() {
    this.dialogRef.close()
  }
}
