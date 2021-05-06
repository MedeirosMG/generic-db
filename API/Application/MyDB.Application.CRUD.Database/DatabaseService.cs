using Microsoft.Extensions.Configuration;
using MyDB.Application.CRUD.DatabaseService.Exceptions;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Cache.Utilities;
using MyDB.Infrastructure.Tools.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MyDB.Application.CRUD.DatabaseService
{
    public class DatabaseService : IDatabaseService
    {
        #region Constructor / Attributes
        private ICacheService _cacheService { get; set; }
        private IConfiguration _config { get; set; }
        private IUtilityService _utilityService { get; set; }
        public DatabaseService(ICacheService cacheService, IConfiguration config, IUtilityService utilityService)
        {
            this._cacheService = cacheService;
            this._config = config;
            this._utilityService = utilityService;
        }
        #endregion

        #region Global Functions
        public string getRegularExpression(string tableId = ".*", string entityId = "\\d*", string attributeId = ".*")
        {
            return $"(.*)({tableId})(\\.)({entityId})(\\.)({attributeId})(.*)";
        }
        #endregion

        #region Database Functions
        /// <summary>
        /// Check if database id is already in use, then create a new database
        /// </summary>
        /// <param name="database"></param>
        private void updateDatabases(Guid database)
        {
            var idDatabases = _config.GetSection("dbManagerKey").Value;
            List<Guid> databases = _cacheService.get<List<Guid>>(idDatabases, database.ToString()) ?? new List<Guid>();

            if (databases.Where(x => x == database)?.Count() != 0)
            {
                _cacheService.del(database);
                throw new GuidColisionException();
            }

            databases.Add(database);
            _cacheService.set(idDatabases, databases, database.ToString());
        }

        /// <summary>
        /// Create a new database, using the current dbName
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public Database createDB(string dbName)
        {
            if (String.IsNullOrEmpty(dbName))
                throw new ValidationErrorException("Invalid database name");

            var database = new Database() { id = Guid.NewGuid(), name = dbName };
            _cacheService.set(database.id, database);
            updateDatabases(database.id);

            return database;
        }

        /// <summary>
        /// Delete database using dbId
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public List<Guid> deleteDB(Guid dbId)
        {
            var idDatabases = _config.GetSection("dbManagerKey").Value;
            List<Guid> databases = _cacheService.get<List<Guid>>(idDatabases) ?? new List<Guid>();

            if (!databases.Contains(dbId))
                throw new DatabaseNotFoundException($"Database {dbId} not exist");

            var database = _cacheService.get<Database>(dbId);
            if (database == null)
                throw new DatabaseNotFoundException($"Database {dbId} not found");

            database.tables.ForEach((item) => { _cacheService.del(item.id); });
            databases.Remove(dbId);

            _cacheService.del(dbId);
            _cacheService.set(idDatabases, databases);

            return databases;
        }

        /// <summary>
        /// Get dabase using current ID
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public Database getDB(Guid dbId, bool resolveReferences = false)
        {
            var database = _cacheService.get<Database>(dbId, dbId.ToString());
            if (database == null)
                throw new DatabaseNotFoundException($"Database {dbId} not found");

            database.tables = database.tables ?? new List<Table>();

            foreach (var idTable in database.tablesId)
            {
                var table = getTable(idTable, resolveReferences);
                if(table == null)
                    throw new TableNotFoundException($"Table {idTable} not found");

                database.tables.Add(table);
            }

            return database;
        }

        /// <summary>
        /// Get all databases ID
        /// </summary>
        /// <returns></returns>
        public List<Guid> getListDB()
        {
            var idDatabases = _config.GetSection("dbManagerKey").Value;
            return _cacheService.get<List<Guid>>(idDatabases) ?? new List<Guid>();
        }
        #endregion

        #region Tables functions
        /// <summary>
        /// Get table using current ID
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public Table getTable(Guid tableId, bool resolveReferences = false)
        {
            var table = _cacheService.get<Table>(tableId, tableId.ToString());
            if (table == null)
                throw new TableNotFoundException($"Table {tableId} not found");

            if (!resolveReferences)
                return table;

            var entities = JArray.Parse(_utilityService.toJson(table.entities ?? new List<dynamic>()));
            entities?.ToList().ForEach((entity) => {
                table.attributes.Where(x => x.referenceAttribute)?.ToList()?.ForEach((attribute) => {
                    var valueReference = getReference(entity[attribute.name].ToString());
                    if(String.IsNullOrEmpty(valueReference))
                        throw new ValidationErrorException($"Reference attribute {attribute.name}: { entity[attribute.name] } notfound");

                    entity[attribute.name] = valueReference;
                });
            });

            var PK = table.attributes.Where(x => x.primaryKey).SingleOrDefault().name;
            entities = new JArray(entities.OrderBy(x => x[PK]));

            var tableEntities = entities.ToString();
            table.entities = _utilityService.fromJson<List<dynamic>>(tableEntities);

            return table;
        }
        /// <summary>
        /// Check and update new id table on database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tableKey"></param>
        private Database addDatabaseTable(Database database, Table table)
        {
            database.tablesId = database.tablesId ?? new List<Guid>();
            if (database.tablesId.Where(x => x == table.id)?.Count() != 0)
            {
                _cacheService.del(table.id);
                throw new GuidColisionException();
            }

            // Update database after validation
            database.tablesId.Add(table.id);
            _cacheService.set(database.id, database);

            return database;
        }
        /// <summary>
        /// Create a new table using the current tableName, inside of specified database
        /// </summary>
        /// <param name="databaseKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Table createTable(Guid databaseKey, string tableName, List<TableAttribute> attributes)
        {
            var table = new Table();
            _cacheService.getLock(() => {
                var database = _cacheService.get<Database>(databaseKey);
                if (database == null)
                    throw new DatabaseNotFoundException($"Database {databaseKey} not found.");

                if (attributes.Where(x => x.primaryKey)?.Count() != 1)
                    throw new ValidationErrorException("Invalid number of primary key");

                if (attributes.Where(x => x.primaryKey).FirstOrDefault().referenceAttribute)
                    throw new ValidationErrorException("Primary key can not be reference attribute.");

                if (attributes.Count() < 2)
                    throw new ValidationErrorException("Database must have at least 2 attributes.");

                attributes.ForEach(x => x.id = Guid.NewGuid());
                table = new Table() { id = Guid.NewGuid(), name = tableName, attributes = attributes };
                _cacheService.set(table.id, table, table.id.ToString());
                addDatabaseTable(database, table);

            }, databaseKey.ToString());

            return table;
        }
        /// <summary>
        /// Delete table using dbId + tableId (Check references tables before)
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public Database deleteTable(Guid dbId, Guid tableId) {
            var database = this.getDB(dbId);

            if (database.tables.Where(x => x.id == tableId)?.Count() == 0)
                throw new TableNotFoundException($"Table {tableId} not found");

            var referenceTables = database.tables.Where(x => Regex.Match(_utilityService.toJson(x.entities), getRegularExpression(tableId.ToString())).Success);
            if (referenceTables?.Count() > 0)
                throw new ValidationErrorException($"Table {tableId} used like reference by: {String.Join(",", referenceTables.Select(x => x.id))}");

            _cacheService.del(tableId);

            var cacheDb = _cacheService.get<Database>(dbId);
            cacheDb.tablesId.RemoveAll(x => x == tableId);
            _cacheService.set(cacheDb.id, cacheDb);

            return getDB(dbId, true);
        }
        #endregion

        #region Entity Fuctions
        private string getReference(string referencePoint)
        {
            var splitedReference = referencePoint.Split(".");
            if (splitedReference.Count() != 3)
                throw new ValidationErrorException("Reference data invalid.");

            var tableId = splitedReference[0];
            var entityId = splitedReference[1];
            var attributeId = splitedReference[2];

            var result = "";
            _cacheService.getLock(() =>
            {
                // Table variables
                var table = _cacheService.get<Table>(tableId);
                var PK = table.attributes.Where(x => x.primaryKey).SingleOrDefault().name;
                var entities = JArray.Parse(_utilityService.toJson(table.entities ?? new List<dynamic>()));

                // Value returned
                var entityFilter = entities.Where(x => x[PK].ToString() == entityId).FirstOrDefault();

                result = entityFilter != null ? entityFilter[attributeId]?.ToString() : "";
            }, tableId);

            return result;
        }
        /// <summary>
        /// Add entity to received table/database, using attributes validation
        /// </summary>
        /// <param name="databaseKey"></param>
        /// <param name="tableKey"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Table addEntity(Guid databaseKey, Guid tableKey, dynamic entity)
        {
            var table = new Table();
            this._cacheService.getLock(() => {
                var database = _cacheService.get<Database>(databaseKey);

                if (database == null)
                    throw new DatabaseNotFoundException($"Database {databaseKey} not found.");

                if (!database.tablesId.Contains(tableKey))
                    throw new DatabaseNotFoundException($"Database {databaseKey} not contain table {tableKey}");

                // Validate table
                table = _cacheService.get<Table>(tableKey);
                if (table == null)
                    throw new DatabaseNotFoundException($"Table {tableKey} not found.");

                table.entities = table.entities ?? new List<dynamic>();

                // Validate object entity
                JObject jsonObject = JObject.Parse(_utilityService.toJson(entity));
                var propertiesEntity = jsonObject.Properties().Select(x => x.Name).ToList();
                var attributes = table.attributes.Select(x => x.name).ToList();

                if (jsonObject == null || propertiesEntity?.Count() == 0 || attributes.Where(x => !propertiesEntity.Contains(x))?.Count() != 0)
                    throw new PropertiesEntityException("Properties entity invalid.");

                // Validate PK
                var PK = table.attributes.Where(x => x.primaryKey).SingleOrDefault().name;
                var entities = JArray.Parse(_utilityService.toJson(table.entities ?? new List<dynamic>()));
                if (entities.Where(x => x[PK].ToString() == jsonObject[PK].ToString()).Count() != 0)
                    throw new ValidationErrorException("PK already in use.");

                // Validate all references on table
                table.attributes.Where(x => x.referenceAttribute)?.ToList().ForEach((item) => {
                    // Get properties where is reference
                    jsonObject.Properties().Where(x => x.Name == item.name)?.ToList().ForEach((itemValue) =>
                    {
                        var valueItem = String.IsNullOrEmpty(itemValue.Value.ToString()) ? "" : itemValue.Value.ToString();
                        // Throw error if reference not found
                        if (String.IsNullOrEmpty(getReference(valueItem)))
                            throw new ValidationErrorException($"Reference attribute {itemValue.Name}: { valueItem } notfound");
                    });
                });


                table.entities.Add(entity);
                _cacheService.set(table.id, table);
            }, tableKey.ToString());

            return table;
        }

        /// <summary>
        /// Check if list of entities is not used by another table
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="database"></param>
        /// <param name="tableId"></param>
        private void checkEntitiesReferences(List<int> entities, Database database, Guid tableId)
        {
            List<string> errorsReference = new List<string>();
            entities.ForEach((entityId) =>
            {
                var referenceTables = database.tables
                                        .Where(x => Regex.Match(_utilityService.toJson(x.entities), getRegularExpression(tableId.ToString(), entityId.ToString())).Success);

                if (referenceTables?.Count() > 0)
                    throw new ValidationErrorException($"EntityId {tableId}.{entityId} used like reference by: {String.Join(",", referenceTables.Select(x => x.id))}");
            });
        }

        private void checkEntitiesExist(List<int> entities, JArray entitiesParsed, string PK)
        {
            entities.ForEach((entityId) =>
            {
                if (entitiesParsed.ToList().Where(x => x[PK]?.ToString() == entityId.ToString())?.Count() == 0)
                    throw new EntityNotFoundException($"Entity Pk {entityId} not found");
            });
        }

        /// <summary>
        /// Delete entity (Check refereces tables before)
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public Table deleteEntity(Guid dbId, Guid tableId, List<int> entities)
        {
            var database = this.getDB(dbId);
            if (database == null)
                throw new DatabaseNotFoundException($"Database {dbId} not found");

            // Check references
            checkEntitiesReferences(entities, database, tableId);

            var table = _cacheService.get<Table>(tableId);
            if (table == null)
                throw new TableNotFoundException($"Table {tableId} not found");

            var entitiesParsed = JArray.Parse(_utilityService.toJson(table.entities ?? new List<dynamic>()));
            var PK = table.attributes.Where(x => x.primaryKey).FirstOrDefault().name;

            // Check if value exist
            checkEntitiesExist(entities, entitiesParsed, PK);

            entities.ForEach((entityId) => {
                var indexEntity = entitiesParsed?.ToList().Select((x, i) => new { entity = x, index = i }).First(x => x.entity[PK]?.ToString() == entityId.ToString())?.index;
                entitiesParsed[indexEntity].Remove();
            });

            table.entities = _utilityService.fromJson<List<dynamic>>(entitiesParsed.ToString());
            _cacheService.set(tableId, table);

            return table;
        }
        #endregion
    }
}
