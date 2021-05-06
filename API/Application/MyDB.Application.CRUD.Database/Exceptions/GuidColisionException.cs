using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Exceptions
{
    public class GuidColisionException : Exception
    {
        public GuidColisionException()
        {
        }

        public GuidColisionException(string message)
            : base(message)
        {
        }

        public GuidColisionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
