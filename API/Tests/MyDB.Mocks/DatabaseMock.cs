using AutoMapper;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class DatabaseMock
    {
        #region Attributes / Constructor
        private Mock<IDatabase> _databaseMock { get; set; }
        public DatabaseMock()
        {
            this._databaseMock = new Mock<IDatabase>();
        }
        #endregion

        #region Mocks
        private void configureMock()
        {
            this._databaseMock
                .Setup(x => x.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .Returns(default(RedisValue));
        }
        public IDatabase getMock()
        {
            this.configureMock();
            return this._databaseMock.Object;
        }
        #endregion
    }
}
