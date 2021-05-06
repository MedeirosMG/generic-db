using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Exceptions
{
    public class RedisException : Exception
    {
        public RedisException()
        {
        }

        public RedisException(string message)
            : base(message)
        {
        }

        public RedisException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
