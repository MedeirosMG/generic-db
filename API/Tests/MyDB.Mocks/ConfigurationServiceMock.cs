using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Mocks
{
    public class ConfigurationServiceMock
    {
        #region Attributes / Constructor
        private ConfigurationBuilder _configuration { get; set; }
        private Dictionary<string, string> _inMemorySettings { get; set; }
        public ConfigurationServiceMock()
        {
            this._configuration = new ConfigurationBuilder();
            this._inMemorySettings = new Dictionary<string, string>();
        }
        #endregion

        #region Mocks
        private void setInMemorySettings()
        {
            this._inMemorySettings = new Dictionary<string, string> {
                {"redisConection", "localhost:6379,ConnectTimeout=5000"},
                {"dbManagerKey", "DATABASE"},
            };

        }
        public IConfiguration getMock()
        {
            this.setInMemorySettings();
            return new ConfigurationBuilder()
                .AddInMemoryCollection(this._inMemorySettings)
                .Build();
        }
        #endregion
    }
}
