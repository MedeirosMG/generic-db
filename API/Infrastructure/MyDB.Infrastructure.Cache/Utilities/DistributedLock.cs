using MyDB.Infrastructure.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MyDB.Infrastructure.Cache.Utilities
{
    public sealed class DistributedLock : IDistributedLock
    {
        private static readonly TimeSpan DefaultAbandonmentCheckFrequency = TimeSpan.FromSeconds(2);

        private readonly string prefixKey = "LOCK";
        private int checkTimeSpan { get; set; }
        private int autoDelete { get; set; }

        public DistributedLock(){
            this.autoDelete = 60000;
            this.checkTimeSpan = 50;
        }


        /// <summary>
        /// Get the lock
        /// </summary>
        /// <param name="timeout">The timeout is null, then try to return once</param>
        /// <returns>Get the lock successfully?</returns>
        public bool Acquire(IDatabase database, string lockName)
        {
            if (String.IsNullOrEmpty(lockName))
                return true;

            bool bLock = false;
            var dtStart = DateTime.Now.Ticks;
            while (!bLock)
            {
                bLock = TryAcquireOnce(database, lockName);
                if (!bLock)
                    Thread.Sleep(this.checkTimeSpan);

                var ts = new TimeSpan(DateTime.Now.Ticks - dtStart);
                if (ts >= TimeSpan.FromSeconds(5))
                {
                    break;
                }
            }

            return bLock;
        }

        public void Release(IDatabase database, string lockName)
        {
            try
            {
                if (String.IsNullOrEmpty(lockName))
                    return;

                database.LockRelease(this.prefixKey + lockName, lockName);
            }
            catch (Exception e)
            {
                throw new RedisException($"Error on release key {lockName}");
            }
        }

        private bool TryAcquireOnce(IDatabase database, string lockName)
        {
            try
            {
                var @lock = database.LockTake(this.prefixKey + lockName, lockName, new TimeSpan(0, 0, 0, 0, this.autoDelete));
                return @lock;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
