import { tableAttribute } from "./tableAttributes";

export interface Table {
  id: string;
  name: string;
  displayedColumns: string[];
  attributes: tableAttribute[];
  entities: any[];
  openState: boolean
}