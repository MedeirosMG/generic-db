using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MyDB.Infrastructure.Cache.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MyDB.Infrastructure.Cache.Interfaces
{
    public class ConnectionFactory : IConnectionFactory
    {
        private IConfiguration _config { get; set; }
        public ConnectionFactory(IConfiguration config)
        {
            this._config = config;
        }
        public IDatabase getDatabase()
        {
            return ConnectionMultiplexer.Connect(_config.GetSection("redisConection").Value).GetDatabase();
        }
    }
}
