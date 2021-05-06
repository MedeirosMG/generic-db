using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Domain.CRUD.DatabaseService
{
    public class Database
    {
        public Database()
        {
            this.tables = new List<Table>();
            this.tablesId = new List<Guid>();
        }
        public Guid id { get; set; }
        public string name { get; set; }
        public List<Table> tables { get; set; }
        public List<Guid> tablesId { get; set; }
    }
}
