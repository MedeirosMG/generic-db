using BaseController;
using Microsoft.AspNetCore.Mvc;
using MyDB.Application.CRUD.Models.Database;
using MyDB.Application.CRUD.Models.Database.Payloads;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Domain.CRUD.DatabaseService;
using MyDB.Infrastructure.BaseController.Applications;
using MyDB.Infrastructure.BaseController.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CRUD
{
    public class DeleteController : CRUDBaseController
    {
        #region Constructor/Attributes
        public DeleteController(IServiceProvider serviceProvider) : base(serviceProvider){ }
        #endregion

        #region Routes
        /// <summary>
        /// Delete database by id
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<APIResponse<List<Guid>>> db(Guid dbId)
        {
            return base.BeginCommonHandler<List<Guid>>(response => {
                response.content = _databaseService.deleteDB(dbId);
            });
        }
        /// <summary>
        /// Delete table using dbId plus tableId
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<APIResponse<DatabaseViewModel>> table(Guid dbId, Guid tableId)
        {
            return base.BeginCommonHandler<DatabaseViewModel>(response => {
                response.content = _mapper.Map<DatabaseViewModel>(_databaseService.deleteTable(dbId, tableId)); 
            });
        }

        /// <summary>
        /// Delete entity using dbId + tableId + entityId
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPost("{dbId}/{tableId}")]
        public ActionResult<APIResponse<TableViewModel>> entity(Guid dbId, Guid tableId, List<int> entities)
        {
            return base.BeginCommonHandler<TableViewModel>(response => {
                response.content = _mapper.Map<TableViewModel>(_databaseService.deleteEntity(dbId, tableId, entities));
            });
        }

        #endregion
    }
}
