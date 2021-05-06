using Moq;
using MyDB.Infrastructure.Cache.Interfaces;
using StackExchange.Redis;
using System;

namespace MyDB.Mocks
{
    public class DistributedLockMock
    {
        #region Attributes / Constructor
        private Mock<IDistributedLock> _distributedLockMock { get; set; }        
        public DistributedLockMock()
        {
            this._distributedLockMock = new Mock<IDistributedLock>();
        }
        #endregion

        #region Mocks
        private void configureMock()
        {
            this._distributedLockMock
                .Setup(x => x.Acquire(It.IsAny<IDatabase>(), It.IsAny<string>()))
                .Returns(true);
        }
        public IDistributedLock getMock()
        {
            configureMock();
            return this._distributedLockMock.Object;
        }
        #endregion
    }
}
