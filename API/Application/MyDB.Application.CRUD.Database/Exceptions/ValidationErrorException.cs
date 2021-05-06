using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Exceptions
{
    public class ValidationErrorException : Exception
    {
        public ValidationErrorException()
        {
        }

        public ValidationErrorException(string message)
            : base(message)
        {
        }

        public ValidationErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
