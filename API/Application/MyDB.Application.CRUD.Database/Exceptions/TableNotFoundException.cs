using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Exceptions
{
    public class TableNotFoundException : Exception
    {
        public TableNotFoundException()
        {
        }

        public TableNotFoundException(string message)
            : base(message)
        {
        }

        public TableNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
