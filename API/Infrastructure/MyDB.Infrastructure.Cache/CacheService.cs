using Microsoft.Extensions.Configuration;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Cache.Utilities;
using MyDB.Infrastructure.Tools.Interfaces;
using StackExchange.Redis;
using System;

namespace MyDB.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        #region Constructor/Attributes
        private IConnectionFactory _connectionFactory { get; set; }
        private IUtilityService _utilityService { get; set; }
        private IDistributedLock _distributedLock { get; set; }
        public CacheService(IConnectionFactory connectionFactory, IUtilityService utilityService, IDistributedLock distributedLock)
        {
            this._connectionFactory = connectionFactory;
            this._utilityService = utilityService;
            this._distributedLock = distributedLock;
        }
        #endregion

        public T set<T>(Guid key, T value, string lockId = "")
        {
            return this.set(key.ToString(), value, lockId);
        }

        public T set<T>(string key, T value, string lockId = "")
        {
            IDatabase dbRedis = null;
            try
            {
                dbRedis = this._connectionFactory.getDatabase();
                if (!this._distributedLock.Acquire(dbRedis, lockId))
                    throw new RedisException($"Timeout for arquire key {lockId} on redis");

                dbRedis.StringSet(key, _utilityService.toJson(value));
                return value;
            }
            finally
            {
                this._distributedLock.Release(dbRedis, lockId);
            }
        }

        public T get<T>(Guid key, string lockId = "")
        {
            return this.get<T>(key.ToString(), lockId);
        }

        public T get<T>(string key, string lockId = "")
        {
            IDatabase dbRedis = null;
            try
            {
                dbRedis = this._connectionFactory.getDatabase();
                if (!this._distributedLock.Acquire(dbRedis, lockId))
                    throw new RedisException($"Timeout for arquire key {lockId} on redis");

                var response = dbRedis.StringGet(key);
                return _utilityService.fromJson<T>(response);
            }
            finally
            {
                this._distributedLock.Release(dbRedis, lockId);
            }
        }
        public bool del(Guid key, string lockId = "")
        {
            return this.del(key.ToString(), lockId);
        }
        public bool del(string key, string lockId = "")
        {
            IDatabase dbRedis = null;
            try
            {
                dbRedis = this._connectionFactory.getDatabase();
                if (!this._distributedLock.Acquire(dbRedis, lockId))
                    throw new RedisException($"Timeout for arquire key {lockId} on redis");

                var deleteKey = dbRedis.KeyDelete(key);
                return deleteKey;
            }
            finally
            {
                this._distributedLock.Release(dbRedis, lockId);
            }
        }

        public void getLock(Action func, string lockId)
        {
            IDatabase dbRedis = null;
            try
            {
                dbRedis = this._connectionFactory.getDatabase();
                if (!this._distributedLock.Acquire(dbRedis, lockId))
                    throw new RedisException($"Timeout for arquire key {lockId} on redis");

                func.Invoke();
            }
            finally
            {
                this._distributedLock.Release(dbRedis, lockId);
            }
        }
    }
}
