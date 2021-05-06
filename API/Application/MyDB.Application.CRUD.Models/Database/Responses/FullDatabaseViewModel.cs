using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.Models.Database.Responses
{
    public class FullDatabaseViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public List<TableViewModel> tables { get; set; }
    }
}
