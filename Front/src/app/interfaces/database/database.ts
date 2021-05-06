import { Table } from "./table";

export interface Database{
    id: string;
    name: string;
    tables: Table[]
  }  