using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.Models.Database.Payloads
{
    public class DataEntityPayload
    {
        public Guid databaseKey { get; set; }
        public Guid tableKey { get; set; }
        public dynamic entity { get; set; }
    }
}
