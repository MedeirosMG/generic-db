import { Component, Input, OnInit } from '@angular/core';
import { Table } from 'src/app/interfaces/database/table';

@Component({
  selector: 'app-generic-table',
  templateUrl: './generic-table.component.html',
  styleUrls: ['./generic-table.component.css']
})
export class GenericTableComponent implements OnInit {

  constructor() { }
  @Input() tables: Table[] = [];

  ngOnInit(): void {
  }

  // Check if fild is reference field
  isReference(attributes, idCampo) {
    return attributes.filter(x => x.name == idCampo)[0].referenceAttribute
  }

  // Get all object propertie
  getProperties(object) {
    if (!object)
      return []

    return Object.getOwnPropertyNames(object)
  }

  // Get object propertie name
  getPropertieName(index, object) {
    if (!object)
      return ""
      
    return Object.getOwnPropertyNames(object)[index]
  }
}
