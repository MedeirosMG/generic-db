using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class MapperServiceMock
    {
        #region Attributes / Constructor
        private Mock<IMapper> _mapperMock { get; set; }
        private MapperConfiguration _mapConfig { get; set; }
        public MapperServiceMock()
        {
            this._mapperMock = new Mock<IMapper>();
            this._mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
        }
        #endregion

        #region Mocks
        public IMapper getMock()
        {
            return this._mapConfig.CreateMapper();
        }
        #endregion
    }
}
