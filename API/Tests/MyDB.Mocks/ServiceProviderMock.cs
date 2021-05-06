using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.Cache;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Tools;
using MyDB.Infrastructure.Tools.Interfaces;
using System;

namespace MyDB.Mocks
{
    public class ServiceProviderMock
    {
        #region Attributes / Constructor
        private Mock<IServiceProvider> _serviceProviderMock { get; set; }
        private Mock<IServiceScope> _serviceScopeMock { get; set; }
        private Mock<IServiceScopeFactory> _serviceScopeFactoryMock { get; set; }
        private CacheServiceMock _cacheServiceMock { get; set; }
        private UtilityServiceMock _utilityServiceMock { get; set; }
        private DatabaseServiceMock _databaseServiceMock { get; set; }
        private MapperServiceMock _mapperServiceMock { get; set; }
        private DistributedLockMock _distributedLockMock { get; set; }
        private ConnectionFactoryMock _connectionFactoryMock { get; set; }
        public ServiceProviderMock()
        {
            this._serviceProviderMock = new Mock<IServiceProvider>();
            this._serviceScopeMock = new Mock<IServiceScope>();
            this._serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            this._cacheServiceMock = new CacheServiceMock();
            this._utilityServiceMock = new UtilityServiceMock();
            this._databaseServiceMock = new DatabaseServiceMock();
            this._mapperServiceMock = new MapperServiceMock();
            this._distributedLockMock = new DistributedLockMock();
            this._connectionFactoryMock = new ConnectionFactoryMock();
        }
        #endregion
        private void configureServiceProvider()
        {
            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IConnectionFactory)))
                .Returns(this._connectionFactoryMock.getMock());

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IDistributedLock)))
                .Returns(this._distributedLockMock.getMock());

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(ICacheService)))
                .Returns(this._cacheServiceMock.getMock());

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IUtilityService)))
                .Returns(this._utilityServiceMock.getMock());            

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(_mapperServiceMock.getMock());

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IDatabaseService)))
                .Returns(this._databaseServiceMock.getMock(this._cacheServiceMock.getMock()));

            this._serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(this._serviceProviderMock.Object);

            this._serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(this._serviceScopeMock.Object);

            this._serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(this._serviceScopeFactoryMock.Object);
        }        
        public IServiceProvider getMock()
        {
            configureServiceProvider();
            return this._serviceProviderMock.Object;
        }
    }
}
