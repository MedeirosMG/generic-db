using CRUD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Infrastructure.BaseController.Model;
using MyDB.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace MyDB.Backend.CRUD.Test
{
    public class QueryControllerTest
    {
        #region Attributes / Constructor
        private ServiceProviderMock _serviceProviderMock { get; set; }
        private QueryController _controller { get; set; }
        public QueryControllerTest()
        {
            this._serviceProviderMock = new ServiceProviderMock();
            this._controller = new QueryController(this._serviceProviderMock.getMock());
            this._controller.ControllerContext = new ControllerContext();
            this._controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
        #endregion

        #region /listDBs Tests
        private ActionResult<APIResponse<List<Guid>>> callListDbs()
        {
            return this._controller.listDBs();
        }

        [Fact]
        public void listDBs_should_return_list_DBs()
        {
            var result = this.callListDbs();

            Assert.NotNull(result.Value);
            Assert.True(result.Value.content.Count > 0);
        }
        #endregion

        #region /dump Tests
        private ActionResult<APIResponse<FullDatabaseViewModel>> callDump(Guid dbId){
            return this._controller.dump(dbId);
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48")]
        public void dump_should_return_dump(Guid dbId)
        {
            var result = this.callDump(dbId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.content.id, dbId);
            Assert.True(result.Value.content.tables.Count == 2);
        }

        [Theory]
        [InlineData("86e76037-f5ec-47cc-a15a-4cdd9a382c23")]
        [InlineData("4d379c7e-fead-41f9-8159-30a224303dd4")]
        public void dump_should_return_db_not_found(Guid dbId)
        {
            var result = this.callDump(dbId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.message, $"Database {dbId} not found");
        }

        [Theory]
        [InlineData("ce458382-6d85-4e04-8564-6215bea0b863")]
        public void dump_should_return_table_not_found(Guid dbId)
        {
            var result = this.callDump(dbId);

            Assert.NotNull(result.Value);
            Assert.True(Regex.Match(result.Value.message, @"Table .* not found").Success);
        }
        #endregion

        #region /fullDb Tests
        private ActionResult<APIResponse<FullDatabaseViewModel>> callFullDb(Guid dbId)
        {
            return this._controller.fullDb(dbId);
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48")]
        public void fullDb_should_return_db(Guid dbId)
        {
            var result = this.callFullDb(dbId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.content.id, dbId);
            Assert.True(result.Value.content.tables.Count == 2);
        }

        [Theory]
        [InlineData("86e76037-f5ec-47cc-a15a-4cdd9a382c23")]
        [InlineData("4d379c7e-fead-41f9-8159-30a224303dd4")]
        public void fullDb_should_return_db_not_found(Guid dbId)
        {
            var result = this.callFullDb(dbId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.message, $"Database {dbId} not found");
        }

        [Theory]
        [InlineData("ce458382-6d85-4e04-8564-6215bea0b863")]
        public void fullDb_should_return_table_not_found(Guid dbId)
        {
            var result = this.callFullDb(dbId);

            Assert.NotNull(result.Value);
            Assert.True(Regex.Match(result.Value.message, @"Table .* not found").Success);
        }

        [Theory]
        [InlineData("6f297c81-2847-49c3-87ea-ca451c185dfa")]
        public void fullDb_should_return_reference_data_invalid(Guid dbId)
        {
            var result = this.callFullDb(dbId);

            Assert.NotNull(result.Value);
            Assert.Matches(result.Value.message, "Reference data invalid.");
        }

        [Theory]
        [InlineData("2332f472-5cd5-4813-86d4-8e0205e5cdd9")]
        public void fullDb_should_return_reference_data_not_found(Guid dbId)
        {
            var result = this.callFullDb(dbId);

            Assert.NotNull(result.Value);
            Assert.True(Regex.Match(result.Value.message, @"Reference attribute .*: .* notfound").Success);
        }
        #endregion

        #region /fullTable Tests
        private ActionResult<APIResponse<TableViewModel>> callFullTable(Guid tableId)
        {
            return this._controller.fullTable(tableId);
        }

        [Theory]
        [InlineData("a99ad182-fadd-4ab5-90b2-66fb67983121")]
        [InlineData("92b5e6f6-98e5-4983-be91-e53afb5e4b41")]
        public void fullTable_should_return_table(Guid tableId)
        {
            var result = this.callFullTable(tableId);

            Assert.NotNull(result.Value);
            Assert.True(result.Value.content.attributes.Count > 0);
            Assert.NotEmpty(result.Value.content.name);
            Assert.Equal(result.Value.content.id, tableId);
        }

        [Theory]
        [InlineData("f49d2631-6fb7-4478-81a4-be8045221cbe")]
        [InlineData("d8953933-4976-437b-9d96-e73fa2f788e5")]
        public void fullTable_should_return_table_not_found(Guid tableId)
        {
            var result = this.callFullTable(tableId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.message, $"Table {tableId} not found");
        }

        [Theory]
        [InlineData("70dccdbc-9d8e-4d0f-9839-39a66b406351")]
        public void fullTable_should_return_invalid_reference(Guid tableId)
        {
            var result = this.callFullTable(tableId);

            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.message, $"Reference data invalid.");
        }

        [Theory]
        [InlineData("3f3ea955-e08e-48b0-8131-bcad0c776412")]
        public void fullTable_should_return_reference_not_found(Guid tableId)
        {
            var result = this.callFullTable(tableId);

            Assert.NotNull(result.Value);
            Assert.True(Regex.Match(result.Value.message, @"Reference attribute .*: .* notfound").Success);
        }
        #endregion
    }
}
