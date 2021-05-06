using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MyDB.Infrastructure.Cache.Interfaces
{
    public interface IConnectionFactory
    {
        IDatabase getDatabase();
    }
}
