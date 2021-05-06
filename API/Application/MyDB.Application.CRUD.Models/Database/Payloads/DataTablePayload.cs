using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.Models.Database.Payloads
{
    public class DataTablePayload
    {
        public Guid databaseKey { get; set; }
        public string tableName { get; set; }
        public List<TableAttributePayload> attributes { get; set; }
    }

    public class TableAttributePayload
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool referenceAttribute { get; set; }
        public bool primaryKey { get; set; }
    }
}
