using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Infrastructure.Cache.Interfaces
{
    public interface ICacheService
    {
        T set<T>(string key, T value, string lockId = "");
        T set<T>(Guid key, T value, string lockId = "");
        T get<T>(string key, string lockId = "");
        T get<T>(Guid key, string lockId = "");
        bool del(Guid key, string lockId = "");
        bool del(string key, string lockId = "");
        void getLock(Action func, string lockId);
    }
}
