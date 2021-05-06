using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MyDB.Infrastructure.Cache.Interfaces
{
    public interface IDistributedLock
    {
        bool Acquire(IDatabase database, string lockName);
        void Release(IDatabase database, string lockName);
    }
}
