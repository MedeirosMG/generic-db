using Microsoft.Extensions.Configuration;
using Moq;
using MyDB.Infrastructure.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class ConnectionFactoryMock
    {
        #region Attributes / Constructor
        private DatabaseMock _databaseMock { get; set; }
        private Mock<IConnectionFactory> _connectionFactoryMock { get; set; }
        public ConnectionFactoryMock()
        {
            this._connectionFactoryMock = new Mock<IConnectionFactory>();
            this._databaseMock = new DatabaseMock();
        }
        #endregion

        #region Mocks
        private void configureMock()
        {
            this._connectionFactoryMock
                .Setup(x => x.getDatabase())
                .Returns(this._databaseMock.getMock());
        }
        public IConnectionFactory getMock()
        {
            this.configureMock();
            return this._connectionFactoryMock.Object;
        }
        #endregion
    }
}
