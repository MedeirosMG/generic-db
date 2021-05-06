using CRUD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Infrastructure.BaseController.Model;
using MyDB.Mocks;
using MyDB.Mocks.MockModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xunit;

namespace MyDB.Backend.CRUD.Test
{
    public class DeleteControllerTest
    {
        #region Attributes / Constructor
        private ServiceProviderMock _serviceProviderMock { get; set; }
        private DeleteController _controller { get; set; }
        public DeleteControllerTest()
        {
            this._serviceProviderMock = new ServiceProviderMock();
            this._controller = new DeleteController(this._serviceProviderMock.getMock());
            this._controller.ControllerContext = new ControllerContext();
            this._controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
        #endregion

        #region /db Tests
        private ActionResult<APIResponse<List<Guid>>> callDeleteDb(Guid guidId)
        {
            return this._controller.db(guidId);
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48")]
        public void db_should_delete(Guid databaseId)
        {
            var response = this.callDeleteDb(databaseId);

            Assert.NotNull(response.Value);
            Assert.True(!response.Value.content.Contains(databaseId));
        }

        [Theory]
        [InlineData("ff8dfe31-5f83-4d15-8d1c-30c01a5f29d3")]
        [InlineData("10fa8fdb-6686-46f0-8c4c-5b25ef2197a3")]
        public void db_should_return_database_not_exist(Guid databaseId)
        {
            var response = this.callDeleteDb(databaseId);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.message, $"Database {databaseId} not exist");
        }

        [Theory]
        [InlineData("d3f3cf47-27fa-4f5c-b236-d01481e7f5f2")]
        [InlineData("837fc16d-405e-4f63-920c-8542c3f5daa1")]
        public void db_should_return_database_not_found(Guid databaseId)
        {
            var response = this.callDeleteDb(databaseId);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.message, $"Database {databaseId} not found");
        }
        #endregion

        #region /table Tests
        private ActionResult<APIResponse<DatabaseViewModel>> callDeleteTable(Guid dbId, Guid tableId)
        {
            return this._controller.table(dbId, tableId);
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48", "92b5e6f6-98e5-4983-be91-e53afb5e4b41")]
        public void table_should_delete_table(Guid dbId, Guid tableId)
        {
            var response = this.callDeleteTable(dbId, tableId);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.content.id, dbId);
            Assert.True(!response.Value.content.tables.Contains(tableId));
        }

        [Theory]
        [InlineData("50d9a7b2-e779-4869-999d-8709c5efc713", "2dddd31f-d25f-4125-9b1e-d5419008b326")]
        public void table_should_return_db_not_found(Guid dbId, Guid tableId)
        {
            var response = this.callDeleteTable(dbId, tableId);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.message, $"Database {dbId} not found");
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48", "2dddd31f-d25f-4125-9b1e-d5419008b326")]
        public void table_should_return_table_not_found(Guid dbId, Guid tableId)
        {
            var response = this.callDeleteTable(dbId, tableId);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.message, $"Table {tableId} not found");
        }

        [Theory]
        [InlineData("2e04ca2d-1762-4675-9850-01480d843f48", "a99ad182-fadd-4ab5-90b2-66fb67983121")]
        public void table_should_return_error_table_reference(Guid dbId, Guid tableId)
        {
            var response = this.callDeleteTable(dbId, tableId);

            Assert.NotNull(response.Value);
            Assert.Contains($"Table {tableId} used like reference by:", response.Value.message);
        }
        #endregion

        #region /entity Tests
        private ActionResult<APIResponse<TableViewModel>> callDeleteEntity(Guid dbId, Guid tableId, List<int> entities)
        {
            return this._controller.entity(dbId, tableId, entities);
        }

        public static IEnumerable<object[]> entityDbNotFound = GenericMock.getMock<MockDeleteEntities>("DeleteController\\entity\\dbNotFound");
        public static IEnumerable<object[]> entityReferenceError = GenericMock.getMock<MockDeleteEntities>("DeleteController\\entity\\referenceError");
        public static IEnumerable<object[]> entityTableNotFound = GenericMock.getMock<MockDeleteEntities>("DeleteController\\entity\\tableNotFound");
        public static IEnumerable<object[]> entityEntitieNotFound = GenericMock.getMock<MockDeleteEntities>("DeleteController\\entity\\entitieNotFound");
        public static IEnumerable<object[]> entityDelete = GenericMock.getMock<MockDeleteEntities>("DeleteController\\entity\\entityDelete");

        [Theory]
        [MemberData(nameof(entityDelete))]
        public void entity_should_delete(MockDeleteEntities payload)
        {
            var response = this.callDeleteEntity(payload.dbId, payload.tableId, payload.entities);
            Assert.NotNull(response.Value);

            JArray entities = JArray.Parse(JsonSerializer.Serialize(response.Value.content.entities ?? new List<dynamic>()));
            Assert.True(entities.Count == 0);
            Assert.Equal(response.Value.content.id, payload.tableId);
        }

        [Theory]
        [MemberData(nameof(entityDbNotFound))]
        public void entity_should_return_db_not_found(MockDeleteEntities payload)
        {
            var response = this.callDeleteEntity(payload.dbId, payload.tableId, payload.entities);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.message, $"Database {payload.dbId} not found");
        }

        [Theory]
        [MemberData(nameof(entityTableNotFound))]
        public void entity_should_return_reference_error(MockDeleteEntities payload)
        {
            var response = this.callDeleteEntity(payload.dbId, payload.tableId, payload.entities);

            Assert.NotNull(response.Value);
            Assert.Contains(response.Value.message, $"Table {payload.tableId} not found");
        }

        [Theory]
        [MemberData(nameof(entityEntitieNotFound))]
        public void entity_should_return_entity_not_found(MockDeleteEntities payload)
        {
            var response = this.callDeleteEntity(payload.dbId, payload.tableId, payload.entities);

            Assert.NotNull(response.Value);
            Assert.Contains(response.Value.message, $"Entity Pk {payload.entities[0]} not found");
        }
        #endregion
    }
}
