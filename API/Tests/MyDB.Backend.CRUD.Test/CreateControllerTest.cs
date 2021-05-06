using AutoMapper;
using CRUD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyDB.Application.CRUD.DatabaseService;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Application.CRUD.Models.Database.Payloads;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Infrastructure.BaseController.Model;
using MyDB.Infrastructure.Cache;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Tools;
using MyDB.Infrastructure.Tools.Interfaces;
using MyDB.Mocks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace MyDB.Backend.CRUD.Test
{
    public class CreateControllerTest
    {
        #region Attributes / Constructor
        private ServiceProviderMock _serviceProviderMock { get; set; }
        private CreateController _controller { get; set; }
        public CreateControllerTest()
        {
            this._serviceProviderMock = new ServiceProviderMock();
            this._controller = new CreateController(this._serviceProviderMock.getMock());
            this._controller.ControllerContext = new ControllerContext();
            this._controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }
        #endregion

        #region /db Tests
        private ActionResult<APIResponse<DatabaseViewModel>> callCreateDb(string dbName)
        {
            return this._controller.db(dbName);
        }

        [Theory]
        [InlineData("Database With Spaces")]
        [InlineData("DatabaseWithNoSpaces")]
        public void db_should_create_db(string dbName)
        {
            var response = callCreateDb(dbName);
            Assert.NotNull(response.Value);
            Assert.Equal(dbName, response.Value.content.name);
            Assert.True(response.Value.content.tables.Count == 0);
        }

        [Theory]
        [InlineData("")]
        public void db_should_return_validation_error(string dbName)
        {
            var response = callCreateDb(dbName);

            Assert.NotNull(response.Value);
            Assert.Equal("Invalid database name", response.Value.message);
        }
        #endregion

        #region /table Tests
        public static IEnumerable<object[]> tableCreateWithNoSpaces = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\tableCreate.noSpaces");
        public static IEnumerable<object[]> tableCreateWithSpaces = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\tableCreate.withSpaces");
        public static IEnumerable<object[]> tableCreateWithReference = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\tableCreate.withReference");
        public static IEnumerable<object[]> tableQuantityAttributesError = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\quantityAttributesError");
        public static IEnumerable<object[]> tableDbNotFoundMock = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\dbNotFound");
        public static IEnumerable<object[]> tablePkErrorMock = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\pkError");
        public static IEnumerable<object[]> tablePkReferenceErrorMock = GenericMock.getMock<DataTablePayload>("CreateController\\Table\\pkReferenceError");

        private ActionResult<APIResponse<TableViewModel>> callCreateTable(DataTablePayload payload)
        {
            return this._controller.table(payload);
        }

        [Theory]
        [MemberData(nameof(tableCreateWithNoSpaces))]
        [MemberData(nameof(tableCreateWithSpaces))]
        [MemberData(nameof(tableCreateWithReference))]
        public void table_should_create_table(DataTablePayload db)
        {
            var response = callCreateTable(db);

            Assert.NotNull(response.Value);
            Assert.Equal(response.Value.content.name, db.tableName);
            Assert.Equal(response.Value.content.attributes.Count(), db.attributes.Count());

            foreach (var item in db.attributes)
            {
                var responseExpected = response.Value.content.attributes.Where(x => x.name == item.name).SingleOrDefault();

                Assert.NotNull(responseExpected);
                Assert.Equal(responseExpected.name, item.name);
                Assert.Equal(responseExpected.primaryKey, item.primaryKey);
                Assert.Equal(responseExpected.referenceAttribute, item.referenceAttribute);
                Assert.Equal(responseExpected.type, item.type);
            }
        }

        [Theory]
        [MemberData(nameof(tableDbNotFoundMock))]
        public void table_should_return_db_not_found(DataTablePayload db)
        {
            var response = callCreateTable(db);

            Assert.NotNull(response.Value);
            Assert.Equal($"Database {db.databaseKey} not found.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(tableQuantityAttributesError))]
        public void table_should_return_quantity_error(DataTablePayload db)
        {
            var response = callCreateTable(db);

            Assert.NotNull(response.Value);
            Assert.Equal("Database must have at least 2 attributes.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(tablePkErrorMock))]
        public void table_should_return_pk_not_found(DataTablePayload db)
        {
            var response = callCreateTable(db);

            Assert.NotNull(response.Value);
            Assert.Equal("Invalid number of primary key", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(tablePkReferenceErrorMock))]
        public void table_should_return_pk_reference_error(DataTablePayload db)
        {
            var response = callCreateTable(db);

            Assert.NotNull(response.Value);
            Assert.Equal("Primary key can not be reference attribute.", response.Value.message);
        }
        #endregion

        #region /entity Tests
        public static IEnumerable<object[]> entityCreate = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\entityCreate");
        public static IEnumerable<object[]> entityDbNotFound = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\dbNotFound");
        public static IEnumerable<object[]> entityTableNotContain = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\tableNotContain");
        public static IEnumerable<object[]> entityTableNotFound = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\tableNotFound");
        public static IEnumerable<object[]> entityInvalidProperties = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\invalidProperties");
        public static IEnumerable<object[]> entityErrorPkInUse = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\pkInUse");
        public static IEnumerable<object[]> entityErrorReferenceNotFound = GenericMock.getMock<DataEntityPayload>("CreateController\\Entity\\referenceNotFound");
        private ActionResult<APIResponse<TableViewModel>> callCreateEntity(DataEntityPayload payload)
        {
            return this._controller.entity(payload);
        }

        [Theory]
        [MemberData(nameof(entityCreate))]
        public void entity_should_create(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);
            Assert.NotNull(response.Value);
            var entitiesParsed = JArray.Parse(JsonSerializer.Serialize(response.Value.content.entities ?? new List<dynamic>()));

            Assert.True(entitiesParsed.Count() > 0);
            Assert.Equal(response.Value.content.id, entity.tableKey);
        }

        [Theory]
        [MemberData(nameof(entityDbNotFound))]
        public void entity_should_return_db_not_found(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);

            Assert.NotNull(response.Value);
            Assert.Equal($"Database {entity.databaseKey} not found.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(entityTableNotContain))]
        public void entity_should_return_table_not_contain(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);

            Assert.NotNull(response.Value);
            Assert.Equal($"Database {entity.databaseKey} not contain table {entity.tableKey}", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(entityTableNotFound))]
        public void entity_should_return_table_not_found(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);

            Assert.NotNull(response.Value);
            Assert.Equal($"Table {entity.tableKey} not found.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(entityInvalidProperties))]
        public void entity_should_return_invalid_properties(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);

            Assert.NotNull(response.Value);
            Assert.Equal("Properties entity invalid.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(entityErrorPkInUse))]
        public void entity_should_return_error_pk_in_use(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);

            Assert.NotNull(response.Value);
            Assert.Equal("PK already in use.", response.Value.message);
        }

        [Theory]
        [MemberData(nameof(entityErrorReferenceNotFound))]
        public void entity_should_return_error_reference_not_found(DataEntityPayload entity)
        {
            var response = callCreateEntity(entity);
            JObject parsedObject = JObject.Parse(JsonSerializer.Serialize(entity.entity));
            var objectReference = parsedObject.Properties().Where(x => x.Value.ToString().Split(".").Count() == 3).FirstOrDefault();
            
            Assert.NotNull(response.Value);
            Assert.NotNull(objectReference);
            Assert.Equal($"Reference attribute {objectReference.Name}: { objectReference.Value } notfound", response.Value.message);
        }
        #endregion
    }
}
