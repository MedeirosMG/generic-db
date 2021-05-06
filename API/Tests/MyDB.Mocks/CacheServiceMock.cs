using Microsoft.Extensions.Configuration;
using Moq;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.Cache;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Tools.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MyDB.Mocks
{
    public class CacheServiceMock
    {
        #region Attributes / Constructor
        private Mock<ICacheService> _cacheServiceMock { get; set; }        
        private UtilityServiceMock _utilityServiceMock { get; set; }
        private ConnectionFactoryMock _connectionFactoryMock { get; set; }
        private DistributedLockMock _distributedLockMock { get; set; }
        public CacheServiceMock()
        {
            this._connectionFactoryMock = new ConnectionFactoryMock();
            this._utilityServiceMock = new UtilityServiceMock();
            this._distributedLockMock = new DistributedLockMock();
            this._cacheServiceMock = new Mock<CacheService>(
                this._connectionFactoryMock.getMock(),
                this._utilityServiceMock.getMock(), 
                this._distributedLockMock.getMock()).As<ICacheService>();
        }
        #endregion

        #region Mocks
        private void configureMock()
        {
            var listData = JArray.Parse(File.ReadAllText("ConfigsMock\\MockRedis.json"));
            listData.ToList().ForEach((item) => {

                if(item["type"].ToString() == "database")
                {
                    this._cacheServiceMock
                        .Setup(x => x.get<Database>(item["key"].ToString(), It.IsAny<string>()))
                        .Returns(JsonSerializer.Deserialize<Database>(item["value"].ToString()));

                    this._cacheServiceMock
                        .Setup(x => x.get<Database>(Guid.Parse(item["key"].ToString()), It.IsAny<string>()))
                        .Returns(JsonSerializer.Deserialize<Database>(item["value"].ToString()));
                }else if (item["type"].ToString() == "table")
                {
                    this._cacheServiceMock
                        .Setup(x => x.get<Table>(item["key"].ToString(), It.IsAny<string>()))
                        .Returns(JsonSerializer.Deserialize<Table>(item["value"].ToString()));

                    this._cacheServiceMock
                        .Setup(x => x.get<Table>(Guid.Parse(item["key"].ToString()), It.IsAny<string>()))
                        .Returns(JsonSerializer.Deserialize<Table>(item["value"].ToString()));
                }else if (item["type"].ToString() == "listDB")
                {
                    var listGuid = JsonSerializer.Deserialize<List<string>>(item["value"].ToString());
                    var parsedList = listGuid.Select(x => Guid.Parse(x)).ToList();

                    this._cacheServiceMock
                        .Setup(x => x.get<List<Guid>>(item["key"].ToString(), It.IsAny<string>()))
                        .Returns(parsedList);
                }

                this._cacheServiceMock
                        .Setup(x => x.del(It.IsAny<Guid>(), It.IsAny<string>()))
                        .Returns(true);

                this._cacheServiceMock
                        .Setup(x => x.del(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(true);
            });
        }
        public ICacheService getMock()
        {
            configureMock();
            this._cacheServiceMock.CallBase = true;
            return this._cacheServiceMock.Object;
        }
        #endregion
    }
}
