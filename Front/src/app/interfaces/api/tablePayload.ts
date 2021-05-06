import { tableAttributePayload } from "./tableAttributePayload";

export interface tablePayload {
  databaseKey: string;
  tableName: string;
  attributes: tableAttributePayload[];
}