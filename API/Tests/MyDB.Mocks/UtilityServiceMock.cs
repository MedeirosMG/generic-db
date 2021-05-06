using Moq;
using MyDB.Infrastructure.Tools;
using MyDB.Infrastructure.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class UtilityServiceMock
    {
        #region Attributes / Constructor
        private UtilityService _utilityService { get; set; }
        public UtilityServiceMock()
        {
            this._utilityService = new UtilityService();
        }
        #endregion

        #region Mocks
        public IUtilityService getMock()
        {
            return this._utilityService;
        }
        #endregion
    }
}
