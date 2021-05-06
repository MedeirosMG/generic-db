using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Exceptions
{
    public class PropertiesEntityException : Exception
    {
        public PropertiesEntityException()
        {
        }

        public PropertiesEntityException(string message)
            : base(message)
        {
        }

        public PropertiesEntityException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
