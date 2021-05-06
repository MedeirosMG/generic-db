using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.Models.Database.Responses
{
    public class TableViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public List<TableAttributeViewModel> attributes { get; set; }
        public List<dynamic> entities { get; set; }
    }

    public class TableAttributeViewModel
    {
        public bool primaryKey { get; set; }
        public bool referenceAttribute { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
