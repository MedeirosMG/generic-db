using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Domain.CRUD.DatabaseService
{
    public class Table
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public List<TableAttribute> attributes { get; set; }
        public List<dynamic> entities { get; set; }
    }

    public class TableAttribute
    {
        public TableAttribute() => this.referenceAttribute = false;
        public Guid id { get; set; }
        public bool referenceAttribute { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool primaryKey { get; set; }
    }
}
