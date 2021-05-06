using MyDB.Domain.CRUD.DatabaseService;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Application.CRUD.DatabaseService.Interfaces
{
    public interface IDatabaseService
    {
        #region DB Functions
        Database createDB(string dbName);
        Database getDB(Guid dbId, bool resolveReferences = false);
        List<Guid> getListDB();
        List<Guid> deleteDB(Guid dbId);
        #endregion

        #region Table Functions
        Table getTable(Guid tableId, bool resolveReferences = false);
        Table createTable(Guid databaseKey, string tableName, List<TableAttribute> attributes);
        Database deleteTable(Guid dbId, Guid tableId);
        #endregion

        #region Entity Functions
        Table addEntity(Guid databaseKey, Guid tableKey, dynamic entity);
        Table deleteEntity(Guid dbId, Guid tableId, List<int> entities);
        #endregion
    }
}
