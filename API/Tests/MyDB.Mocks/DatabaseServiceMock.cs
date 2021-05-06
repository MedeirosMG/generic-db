using Microsoft.Extensions.Configuration;
using Moq;
using MyDB.Application.CRUD.DatabaseService;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class DatabaseServiceMock
    {
        #region Attributes / Constructor
        private Mock<IDatabaseService> _databaseServiceMock { get; set; }
        private ConfigurationServiceMock _configurationServiceMock { get; set; }
        private UtilityServiceMock _utilityServiceMock { get; set; }
        public DatabaseServiceMock()
        {
            this._databaseServiceMock = new Mock<IDatabaseService>();
            this._configurationServiceMock = new ConfigurationServiceMock();
            this._utilityServiceMock = new UtilityServiceMock();
        }
        #endregion

        #region Mocks
        public DatabaseService getMock(ICacheService cacheService)
        {
            return new DatabaseService(
                cacheService,
                this._configurationServiceMock.getMock(),
                this._utilityServiceMock.getMock());
        }
        #endregion
    }
}
